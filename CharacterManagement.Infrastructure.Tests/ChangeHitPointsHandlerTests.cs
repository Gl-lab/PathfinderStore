using CharacterManagement.Infrastructure.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class ChangeHitPointsHandlerTests
{
    [Fact]
    public async Task Handle_DamageAfterTemporaryGrant_AbsorbsTemporaryAndPersistsCurrentHitPoints()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 410 );
        DraftCharacter character = await CreateCompletedFighterAsync( dbContext, account.Id );
        ChangeHitPointsHandler handler = CreateHandler( dbContext );

        await handler.Handle(
            new ChangeHitPointsCommand( account.UserId, character.Id, HitPointOperation.GrantTemporary, 5 ),
            CancellationToken.None );
        CharacterHitPointStateDto result = await handler.Handle(
            new ChangeHitPointsCommand( account.UserId, character.Id, HitPointOperation.ApplyDamage, 8 ),
            CancellationToken.None );

        Assert.Equal( 15, result.Current );
        Assert.Equal( 0, result.Temporary );
        Assert.Equal( 18, result.Maximum );
        DraftCharacter persisted = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( item => item.Id == character.Id );
        Assert.Equal( 15, persisted.CurrentHitPoints );
        Assert.Equal( 0, persisted.TemporaryHitPoints );
    }

    [Fact]
    public async Task Handle_CharacterBelongsToAnotherUser_ThrowsAndDoesNotChangeState()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 510 );
        DraftCharacter character = await CreateCompletedFighterAsync( dbContext, account.Id );
        ChangeHitPointsHandler handler = CreateHandler( dbContext );

        await Assert.ThrowsAsync<CharacterManagementException>( async () => await handler.Handle(
            new ChangeHitPointsCommand( 999, character.Id, HitPointOperation.ApplyDamage, 1 ),
            CancellationToken.None ) );

        DraftCharacter persisted = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( item => item.Id == character.Id );
        Assert.Null( persisted.CurrentHitPoints );
        Assert.Equal( 0, persisted.TemporaryHitPoints );
    }

    [Fact]
    public async Task Handle_DraftCharacter_ThrowsAndDoesNotChangeState()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 610 );
        DraftCharacter character = DraftCharacter.Create( account.Id, "Draft", AncestryType.Human );
        dbContext.Character.Add( character );
        dbContext.Entry( character )
            .Property( item => item.SelectedClassId )
            .CurrentValue = "class.fighter";
        await dbContext.SaveChangesAsync();
        ChangeHitPointsHandler handler = CreateHandler( dbContext );

        await Assert.ThrowsAsync<Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException>(
            async () => await handler.Handle(
                new ChangeHitPointsCommand(
                    account.UserId,
                    character.Id,
                    HitPointOperation.ApplyDamage,
                    1 ),
                CancellationToken.None ) );

        Assert.Null( character.CurrentHitPoints );
        Assert.Equal( 0, character.TemporaryHitPoints );
    }

    private static ChangeHitPointsHandler CreateHandler( CharacterManagementDbContext dbContext )
    {
        return new ChangeHitPointsHandler(
            new CharacterRepository( dbContext ),
            new AncestryRepository(),
            new CharacterClassRepository(),
            new TestUnitOfWork( dbContext ) );
    }

    private static async Task<Account> CreateAccountAsync(
        CharacterManagementDbContext dbContext,
        int userId )
    {
        Account account = new Account
        {
            UserId = userId,
            Name = "Name",
            Surname = "Surname",
        };
        dbContext.Account.Add( account );
        await dbContext.SaveChangesAsync();
        return account;
    }

    private static async Task<DraftCharacter> CreateCompletedFighterAsync(
        CharacterManagementDbContext dbContext,
        int accountId )
    {
        DraftCharacter character = DraftCharacter.Create( accountId, "Fighter", AncestryType.Human );
        dbContext.Character.Add( character );
        dbContext.Entry( character )
            .Property( item => item.SelectedClassId )
            .CurrentValue = "class.fighter";
        character.FinalizeCreation( DateTimeOffset.UtcNow );
        await dbContext.SaveChangesAsync();
        return character;
    }
}
