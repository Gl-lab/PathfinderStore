using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Application.Avatars;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
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
            Gender = CharacterGender.Male,
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
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice(
                    "class.fighter.skill.acrobatics_or_athletics",
                    "skill.athletics",
                    null ),
            ],
            AdditionalClassTrainingChoices =
            [
                .. GeneralSkillChoices(
                    "skill.arcana",
                    "skill.crafting",
                    "skill.deception" ),
                new ClassTrainingTargetChoice( null, "Warfare" ),
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
        Assert.Equal( character.Gender, savedCharacter.Gender );
        Assert.Equal( AvatarIds.Unknown, savedCharacter.AvatarId );
        Assert.Equal( character.AncestryType, savedCharacter.AncestryType );
        Assert.Equal( character.HeritageId, savedCharacter.SelectedHeritageId );
        Assert.Equal( character.AncestryFeatId, savedCharacter.SelectedAncestryFeatId );
        Assert.Equal( character.BackgroundId, savedCharacter.SelectedBackgroundId );
        Assert.Equal( character.BackgroundRestrictedBoost, savedCharacter.SelectedBackgroundRestrictedBoost );
        Assert.Equal( character.BackgroundFreeBoost, savedCharacter.SelectedBackgroundFreeBoost );
        Assert.Equal( character.ClassId, savedCharacter.SelectedClassId );
        Assert.Equal( character.ClassKeyAbility, savedCharacter.SelectedClassKeyAbility );
        Assert.Equal( character.FinalFreeBoosts, savedCharacter.AppliedFinalFreeBoosts );
        Assert.Contains( savedCharacter.TrainedSkills, training => training.SkillId == "skill.acrobatics" );
        Assert.Contains( savedCharacter.TrainedSkills, training => training.SkillId == "skill.athletics" );
        Assert.Contains( savedCharacter.TrainedLore, training => training.LoreId == "lore.circus" );
        Assert.Contains( savedCharacter.TrainedLore, training =>
            ( training.LoreId == "lore.custom.warfare" ) &&
            ( training.SourceGrantId == "class.fighter.skill.additional" ) );
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
            DruidicOrderId = "druidic_order.leaf",
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            ],
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice( "class.druid.skill.nature", null, null ),
                new ClassSkillGrantChoice( "druidic_order.leaf.skill.order", null, null ),
            ],
            AdditionalClassTrainingChoices = GeneralSkillChoices(
                "skill.arcana",
                "skill.athletics",
                "skill.crafting",
                "skill.deception" ),
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Contains( savedCharacter.TrainedSkills, training => training.SkillId == "skill.occultism" );
        Assert.Contains( savedCharacter.TrainedSkills, training => training.SkillId == "skill.nature" );
        Assert.Contains( savedCharacter.TrainedSkills, training => training.SkillId == "skill.diplomacy" );
        Assert.Equal( "druidic_order.leaf", savedCharacter.SelectedDruidicOrderId );
        TrainedLore lore = Assert.Single( savedCharacter.TrainedLore );
        Assert.Equal( "lore.custom.ancient_forest", lore.LoreId );
        Assert.Equal( "Ancient Forest Lore", lore.Name );
    }

    [Fact]
    public async Task Handle_Rogue_PersistsRacketAndResolvedTraining()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 55, "Merisiel", "Shadow" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Merisiel",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Dexterity, AbilityType.Intelligence ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.rogue",
            ClassKeyAbility = AbilityType.Dexterity,
            RogueRacketId = "rogue_racket.thief",
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Constitution,
                AbilityType.Intelligence,
                AbilityType.Wisdom,
            ],
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice( "class.rogue.skill.stealth", null, null ),
            ],
            AdditionalClassTrainingChoices = GeneralSkillChoices(
                "skill.arcana",
                "skill.athletics",
                "skill.crafting",
                "skill.deception",
                "skill.diplomacy",
                "skill.intimidation",
                "skill.medicine",
                "skill.nature",
                "skill.occultism" ),
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Equal( "rogue_racket.thief", savedCharacter.SelectedRogueRacketId );
        Assert.Contains( savedCharacter.TrainedSkills, skill =>
            skill.SkillId == "skill.stealth" &&
            skill.SourceGrantId == "class.rogue.skill.stealth" );
        Assert.Contains( savedCharacter.TrainedSkills, skill =>
            skill.SkillId == "skill.thievery" &&
            skill.SourceGrantId == "rogue_racket.thief.skill.thievery" );
    }

    [Fact]
    public async Task Handle_Bard_PersistsSelectedMuse()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 58, "Lem", "Storyteller" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Lem",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Intelligence, AbilityType.Charisma ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Constitution,
            ClassId = "class.bard",
            ClassKeyAbility = AbilityType.Charisma,
            BardMuseId = "bard_muse.maestro",
            BardCantripIds =
            [
                "spell.daze",
                "spell.detect_magic",
                "spell.forbidding_ward",
                "spell.guidance",
                "spell.light",
            ],
            BardSpellIds = [ "spell.command", "spell.fear" ],
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            ],
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice( "class.bard.skill.occultism", null, null ),
                new ClassSkillGrantChoice( "class.bard.skill.performance", null, null ),
            ],
            AdditionalClassTrainingChoices =
            [
                .. GeneralSkillChoices(
                    "skill.athletics",
                    "skill.crafting",
                    "skill.deception",
                    "skill.diplomacy",
                    "skill.intimidation" ),
            ],
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Equal( "bard_muse.maestro", savedCharacter.SelectedBardMuseId );
        Assert.Equal( 5, savedCharacter.BardCantripIds.Count );
        Assert.Equal( [ "spell.command", "spell.fear" ], savedCharacter.BardSpellIds );
        Assert.Contains( savedCharacter.TrainedSkills, training => training.SkillId == "skill.occultism" );
        Assert.Contains( savedCharacter.TrainedSkills, training => training.SkillId == "skill.performance" );
    }

    [Fact]
    public async Task Handle_Witch_PersistsPatronDerivedSpellAndPatronSkill()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 59, "Feiya", "Hexer" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Feiya",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Intelligence, AbilityType.Wisdom ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.witch",
            ClassKeyAbility = AbilityType.Intelligence,
            WitchPatronId = "witch_patron.faiths_flamekeeper",
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Intelligence,
            ],
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice(
                    "witch_patron.faiths_flamekeeper.skill.patron",
                    null,
                    null ),
            ],
            AdditionalClassTrainingChoices = GeneralSkillChoices(
                "skill.arcana",
                "skill.athletics",
                "skill.crafting",
                "skill.deception",
                "skill.diplomacy",
                "skill.intimidation" ),
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Equal( "witch_patron.faiths_flamekeeper", savedCharacter.SelectedWitchPatronId );
        Assert.Equal( "spell.command", savedCharacter.SelectedWitchPatronFamiliarSpellId );
        Assert.Contains( savedCharacter.TrainedSkills, training =>
            ( training.SkillId == "skill.religion" ) &&
            training.SourceGrantId.StartsWith( "witch_patron.", StringComparison.Ordinal ) );
    }

    [Fact]
    public async Task Handle_Ranger_PersistsHuntersEdge()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 57, "Harsk", "Tracker" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Harsk",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Strength, AbilityType.Wisdom ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.ranger",
            ClassKeyAbility = AbilityType.Dexterity,
            HuntersEdgeId = "hunters_edge.precision",
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            ],
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice( "class.ranger.skill.nature", null, null ),
                new ClassSkillGrantChoice( "class.ranger.skill.survival", null, null ),
            ],
            AdditionalClassTrainingChoices = GeneralSkillChoices(
                "skill.arcana",
                "skill.crafting",
                "skill.deception",
                "skill.diplomacy" ),
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Equal( "hunters_edge.precision", savedCharacter.SelectedHuntersEdgeId );
    }

    [Fact]
    public async Task Handle_Wizard_PersistsArcaneSchool()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 59, "Ezren", "Scholar" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Ezren",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Intelligence, AbilityType.Wisdom ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = AbilityType.Dexterity,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.wizard",
            ClassKeyAbility = AbilityType.Intelligence,
            ArcaneSchoolId = "arcane_school.mentalism",
            ArcaneThesisId = "arcane_thesis.spell_substitution",
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Intelligence,
            ],
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice( "class.wizard.skill.arcana", null, null ),
            ],
            AdditionalClassTrainingChoices = GeneralSkillChoices(
                "skill.athletics",
                "skill.crafting",
                "skill.deception",
                "skill.diplomacy",
                "skill.intimidation" ),
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Equal( "arcane_school.mentalism", savedCharacter.SelectedArcaneSchoolId );
        Assert.Equal(
            "arcane_thesis.spell_substitution",
            savedCharacter.SelectedArcaneThesisId );
    }

    [Fact]
    public async Task Handle_CloisteredCleric_PersistsDoctrineDeityAndDomainSelection()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 56, "Kyra", "Dawn" );
        CreateCharacterHandler handler = CreateHandler( dbContext );
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Kyra",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Strength, AbilityType.Wisdom ],
            BackgroundId = "background.acolyte",
            BackgroundRestrictedBoost = AbilityType.Wisdom,
            BackgroundFreeBoost = AbilityType.Charisma,
            ClassId = "class.cleric",
            ClassKeyAbility = AbilityType.Wisdom,
            ClericDoctrineId = "cleric_doctrine.cloistered",
            DeityId = "deity.iomedae",
            ClericDomainId = "domain.might",
            DivineFont = DivineFont.Heal,
            DivineSanctification = DivineSanctification.Holy,
            ClericCantripIds = ClericCantripIds(),
            ClericPreparedSpellIds = [ "spell.heal", "spell.sure_strike" ],
            FinalFreeBoosts =
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            ],
            ClassSkillGrantChoices =
            [
                new ClassSkillGrantChoice(
                    "class.cleric.skill.religion",
                    null,
                    new ClassTrainingTargetChoice( "skill.athletics", null ) ),
            ],
            AdditionalClassTrainingChoices = GeneralSkillChoices(
                "skill.arcana",
                "skill.crafting" ),
        };

        await handler.Handle(
            new CreateCharacterCommand( account.UserId, character ),
            CancellationToken.None );
        dbContext.ChangeTracker.Clear();

        DraftCharacter savedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleAsync( entity => entity.AccountId == account.Id );
        Assert.Equal( "cleric_doctrine.cloistered", savedCharacter.SelectedClericDoctrineId );
        Assert.Equal( "deity.iomedae", savedCharacter.SelectedDeityId );
        Assert.Equal( "domain.might", savedCharacter.SelectedClericDomainId );
        Assert.Equal( DivineFont.Heal, savedCharacter.SelectedDivineFont );
        Assert.Equal( DivineSanctification.Holy, savedCharacter.SelectedDivineSanctification );
        Assert.Equal( ClericCantripIds(), savedCharacter.PreparedClericCantripIds );
        Assert.Equal( [ "spell.heal", "spell.sure_strike" ], savedCharacter.PreparedClericSpellIds );
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
            skillRepository: new SkillRepository(),
            rogueRacketRepository: new RogueRacketRepository(),
            clericDoctrineRepository: new ClericDoctrineRepository(),
            deityRepository: new DeityRepository(),
            huntersEdgeRepository: new HuntersEdgeRepository(),
            druidicOrderRepository: new DruidicOrderRepository(),
            bardMuseRepository: new BardMuseRepository(),
            witchPatronRepository: new WitchPatronRepository(),
            arcaneSchoolRepository: new ArcaneSchoolRepository(),
            arcaneThesisRepository: new ArcaneThesisRepository(),
            clericDomainRepository: new ClericDomainRepository(),
            spellRepository: new SpellRepository() );

        TestUnitOfWork unitOfWork = new TestUnitOfWork( dbContext );

        AvatarSelector avatarSelector = new AvatarSelector(
            new AvatarCatalog(),
            new RandomAvatarSelectionIndexProvider() );
        return new CreateCharacterHandler( accountRepository, characterBuilder, unitOfWork, avatarSelector );
    }

    private static IReadOnlyList<ClassTrainingTargetChoice> GeneralSkillChoices( params string[] skillIds )
    {
        return skillIds
            .Select( skillId => new ClassTrainingTargetChoice( skillId, null ) )
            .ToArray();
    }

    private static string[] ClericCantripIds()
    {
        return [ "spell.daze", "spell.detect_magic", "spell.divine_lance", "spell.guidance", "spell.light" ];
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
