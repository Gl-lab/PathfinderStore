using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class SetCharacterGenderHandlerTests
{
    [Fact]
    public async Task Handle_OwnedLegacyCharacter_PersistsGender()
    {
        await using CharacterManagementDbContext dbContext =
            TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 401 );
        DraftCharacter character = DraftCharacter.Create(
            account.Id,
            "Legacy",
            AncestryType.Human );
        dbContext.Character.Add( character );
        await dbContext.SaveChangesAsync();
        SetCharacterGenderHandler handler = CreateHandler( dbContext );

        await handler.Handle(
            new SetCharacterGenderCommand(
                account.UserId,
                character.Id,
                CharacterGender.Female ),
            CancellationToken.None );

        CharacterGender savedGender = await dbContext.Character
            .AsNoTracking()
            .Where( item => item.Id == character.Id )
            .Select( item => item.Gender )
            .SingleAsync();
        Assert.Equal( CharacterGender.Female, savedGender );
    }

    [Fact]
    public async Task Handle_CharacterOwnedByAnotherUser_ThrowsNotFound()
    {
        await using CharacterManagementDbContext dbContext =
            TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 402 );
        DraftCharacter character = DraftCharacter.Create(
            account.Id,
            "Hidden",
            AncestryType.Human );
        dbContext.Character.Add( character );
        await dbContext.SaveChangesAsync();
        SetCharacterGenderHandler handler = CreateHandler( dbContext );

        await Assert.ThrowsAsync<CharacterManagementException>( () => handler.Handle(
            new SetCharacterGenderCommand( 999, character.Id, CharacterGender.Male ),
            CancellationToken.None ) );
    }

    [Fact]
    public async Task Handle_AlreadySpecifiedCharacter_ThrowsDomainException()
    {
        await using CharacterManagementDbContext dbContext =
            TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 403 );
        DraftCharacter character = DraftCharacter.Create(
            account.Id,
            "Known",
            AncestryType.Human,
            gender: CharacterGender.Male );
        dbContext.Character.Add( character );
        await dbContext.SaveChangesAsync();
        SetCharacterGenderHandler handler = CreateHandler( dbContext );

        await Assert.ThrowsAsync<Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException>(
            () => handler.Handle(
                new SetCharacterGenderCommand(
                    account.UserId,
                    character.Id,
                    CharacterGender.Female ),
                CancellationToken.None ) );
    }

    private static SetCharacterGenderHandler CreateHandler(
        CharacterManagementDbContext dbContext )
    {
        return new SetCharacterGenderHandler(
            new CharacterRepository( dbContext ),
            new TestUnitOfWork( dbContext ) );
    }

    private static async Task<Account> CreateAccountAsync(
        CharacterManagementDbContext dbContext,
        int userId )
    {
        Account account = new Account
        {
            UserId = userId,
            Name = "Test",
            Surname = "User",
        };
        dbContext.Account.Add( account );
        await dbContext.SaveChangesAsync();
        return account;
    }
}
