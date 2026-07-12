using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class CreateCharacterHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsDraftCharacterForCurrentUser()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 52, "Alrik", "Stone" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Thorin",
            Concept = "A dwarf searching for a lost clanhold.",
            Age = 78,
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
        };
        CreateCharacterCommand command = new CreateCharacterCommand( account.UserId, character );

        await handler.Handle( command, CancellationToken.None );

        DraftCharacter? savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleOrDefaultAsync( entity => entity.AccountId == account.Id );

        Assert.NotNull( savedCharacter );
        Assert.Equal( character.Name, savedCharacter.Name );
        Assert.Equal( character.Concept, savedCharacter.Concept );
        Assert.Equal( character.Age, savedCharacter.Age );
        Assert.Equal( character.AncestryType, savedCharacter.AncestryType );
        Assert.Equal( character.HeritageId, savedCharacter.SelectedHeritageId );
        Assert.Equal( character.AncestryFeatId, savedCharacter.SelectedAncestryFeatId );
        Assert.Equal( account.Id, savedCharacter.AccountId );
        Assert.Equal( 12, savedCharacter.AbilityScores.Strength.Value );
        Assert.Equal( 12, savedCharacter.AbilityScores.Intelligence.Value );
        Assert.Equal( 10, savedCharacter.AbilityScores.Dexterity.Value );
    }

    [Fact]
    public async Task Handle_MissingAccount_ThrowsAndDoesNotPersistCharacter()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "MissingAccountCharacter",
            AncestryType = AncestryType.Human,
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
        };
        CreateCharacterCommand command = new CreateCharacterCommand( 404, character );

        await Assert.ThrowsAsync<Pathfinder.CharacterManagement.Application.Exceptions.CharacterManagementException>( async () =>
            await handler.Handle( command, CancellationToken.None ) );

        int characterCount = await dbContext.Character.CountAsync();
        Assert.Equal( 0, characterCount );
    }

    [Fact]
    public async Task Handle_DuplicateFreeBoosts_ThrowsAndDoesNotPersistCharacter()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 53, "Ezren", "Stone" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "InvalidCharacter",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Strength, AbilityType.Strength ],
        };
        CreateCharacterCommand command = new CreateCharacterCommand( account.UserId, character );

        await Assert.ThrowsAsync<Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException>( async () =>
            await handler.Handle( command, CancellationToken.None ) );

        int characterCount = await dbContext.Character.CountAsync();
        Assert.Equal( 0, characterCount );
    }

    private static CreateCharacterHandler CreateHandler( CharacterManagementDbContext dbContext )
    {
        AccountRepository accountRepository = new AccountRepository( dbContext );
        AncestryRepository ancestryRepository = new AncestryRepository();
        CharacterBuilder characterBuilder = new CharacterBuilder( ancestryRepository );
        TestUnitOfWork unitOfWork = new TestUnitOfWork( dbContext );

        return new CreateCharacterHandler( accountRepository, characterBuilder, unitOfWork );
    }

    private static async Task<Account> CreateAccountAsync(
        CharacterManagementDbContext dbContext,
        int userId,
        string name,
        string surname )
    {
        Account account = new Account
        {
            UserId = userId,
            Name = name,
            Surname = surname,
        };

        dbContext.Account.Add( account );
        await dbContext.SaveChangesAsync();

        return account;
    }
}
