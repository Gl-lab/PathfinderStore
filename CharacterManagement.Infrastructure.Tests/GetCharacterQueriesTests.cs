using Pathfinder.CharacterManagement.Application.Converters.Implementation;
using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetCharacterQueriesTests
{
    [Fact]
    public async Task GetCharacters_ReturnsOnlyCharactersForCurrentUser()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account currentUserAccount = await CreateAccountAsync( dbContext, 301 );
        Account anotherUserAccount = await CreateAccountAsync( dbContext, 302 );
        await CreateCharacterAsync( dbContext, currentUserAccount.Id, "Current User Character" );
        await CreateCharacterAsync( dbContext, anotherUserAccount.Id, "Another User Character" );
        GetCharactersHandler handler = CreateListHandler( dbContext );

        IReadOnlyCollection<CharacterDto> result = await handler.Handle( new GetCharactersCommand( currentUserAccount.UserId ), CancellationToken.None );

        CharacterDto character = Assert.Single( result );
        Assert.Equal( "Current User Character", character.Name );
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterBelongsToCurrentUser_ReturnsCharacterDto()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 401 );
        DraftCharacter draftCharacter = await CreateCharacterAsync( dbContext, account.Id, "Merisiel" );
        GetCharacterByIdHandler handler = CreateByIdHandler( dbContext );

        CharacterDto result = await handler.Handle( new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ), CancellationToken.None );

        Assert.Equal( draftCharacter.Id, result.Id );
        Assert.Equal( "Merisiel", result.Name );
        Assert.Equal( 10, result.Characteristics.Strength.Value );
        Assert.Null( result.Backpack );
        Assert.Null( result.BackgroundPackage );
        Assert.Null( result.ClassPackage );
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterHasBackgroundAndClass_ReturnsPersistedPackages()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 402 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        CharacterBuilder builder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository );
        builder.CreateCharacter( account.Id, "Kyra", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acolyte",
            AbilityType.Wisdom,
            AbilityType.Charisma );
        builder.SetClass( "class.cleric", AbilityType.Wisdom );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterRepository characterRepository = new CharacterRepository( dbContext );
        CharacterConvertor characterConvertor = new CharacterConvertor(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            characterRepository,
            characterConvertor );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.BackgroundPackage );
        Assert.Equal( "background.acolyte", result.BackgroundPackage.BackgroundId );
        Assert.Equal( AbilityType.Wisdom, result.BackgroundPackage.RestrictedBoost );
        Assert.Equal( AbilityType.Charisma, result.BackgroundPackage.FreeBoost );
        Assert.Equal( 12, result.Characteristics.Charisma.Value );
        Assert.Contains( result.BackgroundPackage.Grants, grant => grant.Id == "skill.religion" );
        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.cleric", result.ClassPackage.ClassId );
        Assert.Equal( 8, result.ClassPackage.BaseHitPoints );
        Assert.Equal( AbilityType.Wisdom, result.ClassPackage.KeyAbility );
        Assert.Equal( 16, result.Characteristics.Wisdom.Value );
        Assert.Contains( result.ClassPackage.Rules, rule => rule.Id == "class_choice.cleric.deity" );
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterBelongsToAnotherUser_Throws()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account ownerAccount = await CreateAccountAsync( dbContext, 501 );
        DraftCharacter draftCharacter = await CreateCharacterAsync( dbContext, ownerAccount.Id, "Hidden Character" );
        GetCharacterByIdHandler handler = CreateByIdHandler( dbContext );

        await Assert.ThrowsAsync<CharacterManagementException>( async () =>
            await handler.Handle( new GetCharacterByIdCommand( 777, draftCharacter.Id ), CancellationToken.None ) );
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterDoesNotExist_Throws()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 601 );
        GetCharacterByIdHandler handler = CreateByIdHandler( dbContext );

        await Assert.ThrowsAsync<CharacterManagementException>( async () =>
            await handler.Handle( new GetCharacterByIdCommand( account.UserId, 9876 ), CancellationToken.None ) );
    }

    [Fact]
    public async Task GetCharacters_WhenCharacterHasLegacyBrokenAccountReference_ReturnsEmptyCollection()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account currentUserAccount = await CreateAccountAsync( dbContext, 701 );
        DraftCharacter brokenCharacter = DraftCharacter.Create( 701, "Legacy Character", AncestryType.Human );
        dbContext.Character.Add( brokenCharacter );
        await dbContext.SaveChangesAsync();
        GetCharactersHandler handler = CreateListHandler( dbContext );

        IReadOnlyCollection<CharacterDto> result = await handler.Handle( new GetCharactersCommand( currentUserAccount.UserId ), CancellationToken.None );

        Assert.Empty( result );
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterHasLegacyBrokenAccountReference_Throws()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account currentUserAccount = await CreateAccountAsync( dbContext, 801 );
        DraftCharacter brokenCharacter = DraftCharacter.Create( 801, "Legacy Character", AncestryType.Human );
        dbContext.Character.Add( brokenCharacter );
        await dbContext.SaveChangesAsync();
        GetCharacterByIdHandler handler = CreateByIdHandler( dbContext );

        await Assert.ThrowsAsync<CharacterManagementException>( async () =>
            await handler.Handle( new GetCharacterByIdCommand( currentUserAccount.UserId, brokenCharacter.Id ), CancellationToken.None ) );
    }

    private static GetCharactersHandler CreateListHandler( CharacterManagementDbContext dbContext )
    {
        CharacterRepository characterRepository = new CharacterRepository( dbContext );
        CharacterConvertor characterConvertor = new CharacterConvertor();

        return new GetCharactersHandler( characterRepository, characterConvertor );
    }

    private static GetCharacterByIdHandler CreateByIdHandler( CharacterManagementDbContext dbContext )
    {
        CharacterRepository characterRepository = new CharacterRepository( dbContext );
        CharacterConvertor characterConvertor = new CharacterConvertor();

        return new GetCharacterByIdHandler( characterRepository, characterConvertor );
    }

    private static async Task<Account> CreateAccountAsync( CharacterManagementDbContext dbContext, int userId )
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

    private static async Task<DraftCharacter> CreateCharacterAsync(
        CharacterManagementDbContext dbContext,
        int accountId,
        string name )
    {
        DraftCharacter draftCharacter = DraftCharacter.Create( accountId, name, AncestryType.Human );

        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();

        return draftCharacter;
    }
}
