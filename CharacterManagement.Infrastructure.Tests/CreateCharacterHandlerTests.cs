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
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.fighter",
            ClassKeyAbility = AbilityType.Strength,
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom
            ],
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
        Assert.Equal( character.BackgroundId, savedCharacter.SelectedBackgroundId );
        Assert.Equal( character.BackgroundRestrictedBoost, savedCharacter.SelectedBackgroundRestrictedBoost );
        Assert.Equal( character.BackgroundFreeBoost, savedCharacter.SelectedBackgroundFreeBoost );
        Assert.Equal( character.ClassId, savedCharacter.SelectedClassId );
        Assert.Equal( character.ClassKeyAbility, savedCharacter.SelectedClassKeyAbility );
        Assert.Equal( character.FinalFreeBoosts, savedCharacter.AppliedFinalFreeBoosts );
        Assert.Equal( "skill.acrobatics", Assert.Single( savedCharacter.TrainedSkills ).SkillId );
        Assert.Equal( "lore.circus", Assert.Single( savedCharacter.TrainedLore ).LoreId );
        Assert.Equal( account.Id, savedCharacter.AccountId );
        Assert.Equal( 16, savedCharacter.AbilityScores.Strength.Value );
        Assert.Equal( 12, savedCharacter.AbilityScores.Intelligence.Value );
        Assert.Equal( 14, savedCharacter.AbilityScores.Dexterity.Value );
        Assert.Equal( 12, savedCharacter.AbilityScores.Constitution.Value );
        Assert.Equal( 12, savedCharacter.AbilityScores.Wisdom.Value );
        Assert.Equal( 12, savedCharacter.AbilityScores.Charisma.Value );
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
    public async Task Handle_BackgroundChoices_PersistsSelectedSkillAndCustomLore()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 54, "Lini", "Greenbriar" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Lini",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Intelligence, AbilityType.Wisdom ],
            BackgroundId = "background.hermit",
            BackgroundRestrictedBoost = AbilityType.Intelligence,
            BackgroundFreeBoost = AbilityType.Wisdom,
            BackgroundTrainingChoices =
            [
                new BackgroundTrainingChoice(
                    "background.hermit.skill",
                    "skill.occultism",
                    null ),
                new BackgroundTrainingChoice(
                    "background.hermit.lore",
                    null,
                    "Ancient Forest" ),
            ],
            ClassId = "class.druid",
            ClassKeyAbility = AbilityType.Wisdom,
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            ],
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Equal( "skill.occultism", Assert.Single( savedCharacter.TrainedSkills ).SkillId );
        TrainedLore lore = Assert.Single( savedCharacter.TrainedLore );
        Assert.Equal( "lore.custom.ancient_forest", lore.LoreId );
        Assert.Equal( "Ancient Forest Lore", lore.Name );
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
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.fighter",
            ClassKeyAbility = AbilityType.Strength,
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom
            ],
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
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        CharacterBuilder characterBuilder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository() );
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
