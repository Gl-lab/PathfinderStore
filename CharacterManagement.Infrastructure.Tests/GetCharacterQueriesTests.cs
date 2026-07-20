using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.Avatars;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Features.Characters.Queries.Mapping;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
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
        Assert.Equal( CharacterGender.NotSpecified, result.Gender );
        Assert.Equal( 10, result.Characteristics.Strength.Value );
        Assert.Null( result.Backpack );
        Assert.Null( result.BackgroundPackage );
        Assert.Null( result.ClassPackage );
        Assert.Empty( result.FinalFreeBoosts );
        Assert.Null( result.DerivedStatistics );
        Assert.Empty( result.Training.Skills );
        Assert.Empty( result.Training.Lore );
        Assert.Empty( result.Proficiencies );
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterHasBackgroundAndClass_ReturnsPersistedPackages()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 402 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        ClericDoctrineRepository clericDoctrineRepository = new ClericDoctrineRepository();
        DeityRepository deityRepository = new DeityRepository();
        ClericDomainRepository clericDomainRepository = new ClericDomainRepository();
        CharacterBuilder builder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            clericDoctrineRepository: clericDoctrineRepository,
            deityRepository: deityRepository,
            clericDomainRepository: clericDomainRepository,
            spellRepository: new SpellRepository() );
        builder.CreateCharacter( account.Id, "Kyra", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acolyte",
            AbilityType.Wisdom,
            AbilityType.Charisma );
        builder.SetClass(
            "class.cleric",
            AbilityType.Wisdom,
            clericDoctrineId: "cleric_doctrine.cloistered",
            deityId: "deity.iomedae",
            clericDomainId: "domain.might",
            divineFont: DivineFont.Heal,
            divineSanctification: DivineSanctification.Holy,
            clericCantripIds: ClericCantripIds(),
            clericPreparedSpellIds: [ "spell.heal", "spell.sure_strike" ] );
        builder.SetFinalFreeBoosts(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom
            ] );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterRepository characterRepository = new CharacterRepository( dbContext );
        CharacterDetailsDtoMapper characterDetailsDtoMapper = new CharacterDetailsDtoMapper(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository,
            new SkillRepository(),
            clericDoctrineRepository: clericDoctrineRepository,
            deityRepository: deityRepository,
            clericDomainRepository: clericDomainRepository,
            spellRepository: new SpellRepository() );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            characterRepository,
            characterDetailsDtoMapper );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.BackgroundPackage );
        Assert.Equal( "background.acolyte", result.BackgroundPackage.BackgroundId );
        Assert.Equal( AbilityType.Wisdom, result.BackgroundPackage.RestrictedBoost );
        Assert.Equal( AbilityType.Charisma, result.BackgroundPackage.FreeBoost );
        Assert.Equal( 12, result.Characteristics.Charisma.Value );
        Assert.Contains( result.BackgroundPackage.Grants, grant => grant.TargetId == "skill.religion" );
        CharacterSkillTrainingDto trainedSkill = Assert.Single(
            result.Training.Skills,
            skill => skill.Id == "skill.religion" );
        Assert.Equal( "skill.religion", trainedSkill.Id );
        Assert.Equal( "Religion", trainedSkill.Name );
        Assert.Equal( AbilityType.Wisdom, trainedSkill.KeyAbility );
        CharacterLoreTrainingDto trainedLore = Assert.Single( result.Training.Lore );
        Assert.Equal( "lore.scribing", trainedLore.Id );
        Assert.Equal( "Scribing Lore", trainedLore.Name );
        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.cleric", result.ClassPackage.ClassId );
        Assert.Equal( 8, result.ClassPackage.BaseHitPoints );
        Assert.Equal( AbilityType.Wisdom, result.ClassPackage.KeyAbility );
        Assert.Equal( 2, result.ClassPackage.AdditionalSkillCount );
        Assert.NotNull( result.ClassPackage.ClericDoctrine );
        Assert.Equal( "cleric_doctrine.cloistered", result.ClassPackage.ClericDoctrine.Id );
        Assert.NotNull( result.ClassPackage.Deity );
        Assert.Equal( "deity.iomedae", result.ClassPackage.Deity.Id );
        Assert.NotNull( result.ClassPackage.ClericDomain );
        Assert.Equal( "domain.might", result.ClassPackage.ClericDomain.Id );
        Assert.NotNull( result.ClassPackage.ClericSpellLoadout );
        Assert.NotNull( result.ClassPackage.ClericFocusPool );
        Assert.Equal( 1, result.ClassPackage.ClericFocusPool.MaximumFocusPoints );
        Assert.Equal( "spell.athletic_rush", result.ClassPackage.ClericFocusPool.FocusSpell.Id );
        Assert.Equal(
            "cleric_doctrine.cloistered.effect.domain_initiate",
            result.ClassPackage.ClericFocusPool.SourceGrantId );
        Assert.Equal( 5, result.ClassPackage.ClericSpellLoadout.Cantrips.Count );
        Assert.Equal( 2, result.ClassPackage.ClericSpellLoadout.PreparedSpells.Count );
        Assert.Equal( 4, result.ClassPackage.ClericSpellLoadout.DivineFontSpells.Count );
        Assert.Equal(
            [ "spell.heal", "spell.sure_strike" ],
            result.ClassPackage.ClericSpellLoadout.PreparedSpells
                .Select( slot => slot.Spell.Id )
                .ToArray() );
        Assert.All(
            result.ClassPackage.ClericSpellLoadout.DivineFontSpells,
            spell => Assert.Equal( "spell.heal", spell.Id ) );
        Assert.NotSame(
            result.ClassPackage.ClericSpellLoadout.DivineFontSpells[ 0 ],
            result.ClassPackage.ClericSpellLoadout.DivineFontSpells[ 1 ] );
        Assert.Equal(
            "spell.athletic_rush",
            result.ClassPackage.ClericDomain.InitialFocusSpell.Id );
        Assert.Equal( DivineFont.Heal, result.ClassPackage.Deity.DivineFont );
        Assert.Equal( DivineSanctification.Holy, result.ClassPackage.Deity.Sanctification );
        Assert.Equal( "skill.intimidation", result.ClassPackage.Deity.DivineSkillId );
        Assert.Equal( 9, result.Proficiencies.Count );
        Assert.Contains(
            result.Proficiencies,
            proficiency => proficiency.TargetId == "proficiency.attack.weapon.longsword" );
        Assert.Contains(
            result.Proficiencies,
            proficiency =>
                proficiency.TargetId == "proficiency.save.will" &&
                proficiency.Rank == ProficiencyRank.Expert );
        Assert.Contains(
            result.Proficiencies,
            proficiency => proficiency.TargetId == "proficiency.class_dc.cleric" );
        Assert.Equal( 18, result.Characteristics.Wisdom.Value );
        Assert.Contains( result.ClassPackage.Rules, rule => rule.Id == "class_choice.cleric.deity" );
        Assert.Contains( result.Training.Skills, skill => skill.Id == "skill.intimidation" );
        Assert.Equal(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom
            ],
            result.FinalFreeBoosts );
        Assert.NotNull( result.DerivedStatistics );
        Assert.Equal( 17, result.DerivedStatistics.HitPoints.Maximum );
        Assert.Equal( 8, result.DerivedStatistics.HitPoints.Ancestry );
        Assert.Equal( 8, result.DerivedStatistics.HitPoints.Class );
        Assert.Equal( 1, result.DerivedStatistics.HitPoints.ConstitutionModifier );
    }

    [Fact]
    public async Task GetCharacterById_RangerReturnsSelectedHuntersEdge()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 405 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        HuntersEdgeRepository huntersEdgeRepository = new HuntersEdgeRepository();
        CharacterBuilder builder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            huntersEdgeRepository: huntersEdgeRepository );
        builder.CreateCharacter( account.Id, "Harsk", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Charisma );
        builder.SetClass(
            "class.ranger",
            AbilityType.Dexterity,
            huntersEdgeId: "hunters_edge.precision" );
        builder.SetFinalFreeBoosts(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            ] );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository,
            new SkillRepository(),
            huntersEdgeRepository: huntersEdgeRepository );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            new CharacterRepository( dbContext ),
            mapper );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage?.HuntersEdge );
        Assert.Equal( "hunters_edge.precision", result.ClassPackage.HuntersEdge.Id );
        Assert.Equal(
            HuntersEdgeEffectKind.PrecisionDamage,
            Assert.Single( result.ClassPackage.HuntersEdge.Effects ).Kind );
    }

    [Fact]
    public async Task GetCharacterById_LegacyRangerWithoutHuntersEdge_ReturnsNullablePackage()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 406 );
        DraftCharacter draftCharacter = DraftCharacter.Create(
            account.Id,
            "Legacy Harsk",
            AncestryType.Human );
        dbContext.Character.Add( draftCharacter );
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassId )
            .CurrentValue = "class.ranger";
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassKeyAbility )
            .CurrentValue = AbilityType.Dexterity;
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharacterByIdHandler handler = CreateByIdHandler(
            dbContext,
            characterClassRepository: new CharacterClassRepository() );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.ranger", result.ClassPackage.ClassId );
        Assert.Null( result.ClassPackage.HuntersEdge );
    }

    [Fact]
    public async Task GetCharacterById_DruidReturnsSelectedDruidicOrder()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 408 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        DruidicOrderRepository druidicOrderRepository = new DruidicOrderRepository();
        CharacterBuilder builder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            druidicOrderRepository: druidicOrderRepository,
            spellRepository: new SpellRepository() );
        builder.CreateCharacter( account.Id, "Lini", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Charisma );
        builder.SetClass(
            "class.druid",
            AbilityType.Wisdom,
            druidicOrderId: "druidic_order.leaf",
            druidCantripIds: DruidCantripIds(),
            druidPreparedSpellIds: [ "spell.heal", "spell.heal" ] );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository,
            new SkillRepository(),
            druidicOrderRepository: druidicOrderRepository,
            spellRepository: new SpellRepository() );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            new CharacterRepository( dbContext ),
            mapper );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage?.DruidicOrder );
        Assert.Equal( "druidic_order.leaf", result.ClassPackage.DruidicOrder.Id );
        Assert.Equal( "skill.diplomacy", Assert.Single(
            result.ClassPackage.DruidicOrder.SkillGrant.SkillOptions ) );
        Assert.Equal( DruidCantripIds(), result.ClassPackage.DruidSpellLoadout?.Cantrips.Select( spell => spell.Id ) );
        Assert.Equal(
            [ "spell.heal", "spell.heal" ],
            result.ClassPackage.DruidSpellLoadout?.PreparedSpells.Select( spell => spell.Id ) );
        Assert.Equal( "spell.cornucopia", result.ClassPackage.DruidFocusPool?.FocusSpell.Id );
        Assert.Equal( 1, result.ClassPackage.DruidFocusPool?.MaximumFocusPoints );
    }

    [Fact]
    public async Task GetCharacterById_LegacyDruidWithoutOrder_ReturnsNullablePackage()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 409 );
        DraftCharacter draftCharacter = DraftCharacter.Create(
            account.Id,
            "Legacy Lini",
            AncestryType.Human );
        dbContext.Character.Add( draftCharacter );
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassId )
            .CurrentValue = "class.druid";
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassKeyAbility )
            .CurrentValue = AbilityType.Wisdom;
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharacterByIdHandler handler = CreateByIdHandler(
            dbContext,
            characterClassRepository: new CharacterClassRepository() );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.druid", result.ClassPackage.ClassId );
        Assert.Null( result.ClassPackage.DruidicOrder );
    }

    [Fact]
    public async Task GetCharacterById_BardReturnsSelectedMuse()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 410 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        BardMuseRepository bardMuseRepository = new BardMuseRepository();
        CharacterBuilder builder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            bardMuseRepository: bardMuseRepository,
            spellRepository: new SpellRepository() );
        builder.CreateCharacter( account.Id, "Lem", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Charisma ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Constitution );
        builder.SetClass(
            "class.bard",
            AbilityType.Charisma,
            bardMuseId: "bard_muse.enigma",
            bardCantripIds:
            [
                "spell.daze",
                "spell.detect_magic",
                "spell.forbidding_ward",
                "spell.guidance",
                "spell.light",
            ],
            bardSpellIds: [ "spell.command", "spell.fear" ] );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository,
            new SkillRepository(),
            bardMuseRepository: bardMuseRepository,
            spellRepository: new SpellRepository() );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            new CharacterRepository( dbContext ),
            mapper );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage?.BardMuse );
        Assert.Equal( "bard_muse.enigma", result.ClassPackage.BardMuse.Id );
        Assert.Contains(
            result.ClassPackage.BardMuse.Benefits,
            benefit => benefit.Id == "feat.bardic_lore" );
        Assert.NotNull( result.ClassPackage.BardSpellLoadout );
        Assert.Equal( 5, result.ClassPackage.BardSpellLoadout.Cantrips.Count );
        Assert.Equal( 3, result.ClassPackage.BardSpellLoadout.RankOneRepertoire.Count );
        Assert.Contains(
            result.ClassPackage.BardSpellLoadout.RankOneRepertoire,
            spell => spell.Source == BardRepertoireSpellSource.MuseGranted &&
                     spell.Spell.Id == "spell.sure_strike" );
        Assert.Equal( 2, result.ClassPackage.BardSpellLoadout.RankOneSpellSlotCount );
        Assert.NotNull( result.ClassPackage.BardComposition );
        Assert.Equal( "spell.courageous_anthem", result.ClassPackage.BardComposition.CompositionCantrip.Id );
        Assert.Equal( "spell.counter_performance", result.ClassPackage.BardComposition.FocusSpell.Id );
        Assert.Equal( 1, result.ClassPackage.BardComposition.MaximumFocusPoints );
    }

    [Fact]
    public async Task GetCharacterById_LegacyBardWithoutMuse_ReturnsNullablePackage()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 411 );
        DraftCharacter draftCharacter = DraftCharacter.Create(
            account.Id,
            "Legacy Lem",
            AncestryType.Human );
        dbContext.Character.Add( draftCharacter );
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassId )
            .CurrentValue = "class.bard";
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassKeyAbility )
            .CurrentValue = AbilityType.Charisma;
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharacterByIdHandler handler = CreateByIdHandler(
            dbContext,
            characterClassRepository: new CharacterClassRepository() );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.bard", result.ClassPackage.ClassId );
        Assert.Null( result.ClassPackage.BardMuse );
        Assert.Null( result.ClassPackage.BardSpellLoadout );
        Assert.Null( result.ClassPackage.BardComposition );
    }

    [Fact]
    public async Task GetCharacterById_WitchReturnsPatronTraditionAndSelectedFamiliarSpell()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 412 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        WitchPatronRepository patronRepository = new WitchPatronRepository();
        CharacterBuilder builder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            witchPatronRepository: patronRepository,
            spellRepository: new SpellRepository() );
        builder.CreateCharacter( account.Id, "Feiya", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Charisma );
        builder.SetClass(
            "class.witch",
            AbilityType.Intelligence,
            witchPatronId: "witch_patron.wilding_steward",
            witchPatronFamiliarSpellId: "spell.summon_animal",
            witchFamiliarCantripIds: WitchSpellTestData.PrimalCantrips(),
            witchFamiliarSpellIds: WitchSpellTestData.PrimalSpells(),
            witchPreparedCantripIds: WitchSpellTestData.PrimalCantrips().Take( 5 ).ToArray(),
            witchPreparedSpellIds: [ "spell.heal", "spell.heal" ],
            witchFocusHexId: "spell.phase_familiar" );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper(
            ancestryRepository: ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            witchPatronRepository: patronRepository,
            spellRepository: new SpellRepository() );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            new CharacterRepository( dbContext ),
            mapper );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage?.WitchPatron );
        Assert.Equal( SpellTradition.Primal, result.ClassPackage.SpellTradition );
        Assert.Equal( "witch_patron.wilding_steward", result.ClassPackage.WitchPatron.Id );
        Assert.Equal( "spell.summon_animal", result.ClassPackage.WitchPatron.SelectedFamiliarSpell.Id );
        Assert.Equal( 10, result.ClassPackage.WitchSpellLoadout?.FamiliarCantrips.Count );
        Assert.Equal( "spell.summon_animal", result.ClassPackage.WitchSpellLoadout?.PatronGrantedSpell?.Id );
        Assert.Equal( "spell.wilding_word", result.ClassPackage.WitchHexPackage?.PatronHexCantrip?.Id );
        Assert.Equal( "spell.phase_familiar", result.ClassPackage.WitchHexPackage?.FocusHex?.Id );
        Assert.Equal( 1, result.ClassPackage.WitchHexPackage?.MaximumFocusPoints );
    }

    [Fact]
    public async Task GetCharacterById_LegacyWitchWithoutPatron_ReturnsNullablePackage()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 413 );
        DraftCharacter draftCharacter = DraftCharacter.Create(
            account.Id,
            "Legacy Feiya",
            AncestryType.Human );
        dbContext.Character.Add( draftCharacter );
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassId )
            .CurrentValue = "class.witch";
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassKeyAbility )
            .CurrentValue = AbilityType.Intelligence;
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharacterByIdHandler handler = CreateByIdHandler(
            dbContext,
            characterClassRepository: new CharacterClassRepository() );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.witch", result.ClassPackage.ClassId );
        Assert.Null( result.ClassPackage.WitchPatron );
        Assert.Null( result.ClassPackage.SpellTradition );
    }

    [Fact]
    public async Task GetCharacterById_WizardReturnsSelectedArcaneSchool()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 414 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        ArcaneSchoolRepository arcaneSchoolRepository = new ArcaneSchoolRepository();
        ArcaneThesisRepository arcaneThesisRepository = new ArcaneThesisRepository();
        CharacterBuilder builder = new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            arcaneSchoolRepository: arcaneSchoolRepository,
            arcaneThesisRepository: arcaneThesisRepository,
            spellRepository: new SpellRepository() );
        builder.CreateCharacter( account.Id, "Ezren", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Charisma );
        builder.SetClass(
            "class.wizard",
            AbilityType.Intelligence,
            arcaneSchoolId: "arcane_school.mentalism",
            arcaneThesisId: "arcane_thesis.spell_substitution",
            wizardSpellbookCantripIds:
            [
                "spell.detect_magic", "spell.light", "spell.message", "spell.prestidigitation", "spell.read_aura",
                "spell.shield", "spell.sigil", "spell.summon_instrument", "spell.telekinetic_hand", "spell.telekinetic_projectile",
            ],
            wizardSpellbookSpellIds:
            [ "spell.air_bubble", "spell.alarm", "spell.command", "spell.disguise_magic", "spell.enfeeble" ],
            wizardCurriculumCantripId: "spell.daze",
            wizardCurriculumSpellIds: [ "spell.dizzying_colors", "spell.sleep" ],
            wizardPreparedCantripIds:
            [ "spell.detect_magic", "spell.light", "spell.message", "spell.prestidigitation", "spell.read_aura" ],
            wizardPreparedSpellIds: [ "spell.air_bubble", "spell.air_bubble" ],
            wizardPreparedCurriculumCantripId: "spell.figment",
            wizardPreparedCurriculumSpellId: "spell.sure_strike" );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper(
            ancestryRepository: ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository(),
            arcaneSchoolRepository: arcaneSchoolRepository,
            arcaneThesisRepository: arcaneThesisRepository,
            spellRepository: new SpellRepository() );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            new CharacterRepository( dbContext ),
            mapper );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage?.ArcaneSchool );
        Assert.Equal( SpellTradition.Arcane, result.ClassPackage.SpellTradition );
        Assert.Equal( "arcane_school.mentalism", result.ClassPackage.ArcaneSchool.Id );
        Assert.NotEmpty( result.ClassPackage.ArcaneSchool.CurriculumSpells );
        Assert.NotNull( result.ClassPackage.ArcaneThesis );
        Assert.Equal(
            "arcane_thesis.spell_substitution",
            result.ClassPackage.ArcaneThesis.Id );
        Assert.Equal( 10, result.ClassPackage.WizardSpellLoadout?.SpellbookCantrips.Count );
        Assert.Equal( 2, result.ClassPackage.WizardSpellLoadout?.PreparedRankOneSpells.Count );
        Assert.Equal( "spell.sure_strike", result.ClassPackage.WizardSpellLoadout?.PreparedCurriculumRankOneSpell?.Id );
        Assert.Equal( "spell.charming_push", result.ClassPackage.WizardSchoolMagic?.InitialSchoolSpell?.Id );
        Assert.Equal( 1, result.ClassPackage.WizardSchoolMagic?.MaximumFocusPoints );
    }

    [Fact]
    public async Task GetCharacterById_LegacyWizardWithoutSchool_ReturnsNullablePackage()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 415 );
        DraftCharacter draftCharacter = DraftCharacter.Create(
            account.Id,
            "Legacy Ezren",
            AncestryType.Human );
        dbContext.Character.Add( draftCharacter );
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassId )
            .CurrentValue = "class.wizard";
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassKeyAbility )
            .CurrentValue = AbilityType.Intelligence;
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharacterByIdHandler handler = CreateByIdHandler(
            dbContext,
            characterClassRepository: new CharacterClassRepository() );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.wizard", result.ClassPackage.ClassId );
        Assert.Null( result.ClassPackage.ArcaneSchool );
        Assert.Null( result.ClassPackage.ArcaneThesis );
        Assert.Equal( SpellTradition.Arcane, result.ClassPackage.SpellTradition );
    }

    [Fact]
    public async Task GetCharacterById_LegacyWizardWithSchoolWithoutThesis_ReturnsPartialPackage()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 416 );
        DraftCharacter draftCharacter = DraftCharacter.Create(
            account.Id,
            "Pre-Thesis Ezren",
            AncestryType.Human );
        dbContext.Character.Add( draftCharacter );
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassId )
            .CurrentValue = "class.wizard";
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassKeyAbility )
            .CurrentValue = AbilityType.Intelligence;
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedArcaneSchoolId )
            .CurrentValue = "arcane_school.mentalism";
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper(
            characterClassRepository: new CharacterClassRepository(),
            skillRepository: new SkillRepository(),
            arcaneSchoolRepository: new ArcaneSchoolRepository() );
        GetCharacterByIdHandler handler = new GetCharacterByIdHandler(
            new CharacterRepository( dbContext ),
            mapper );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage?.ArcaneSchool );
        Assert.Equal( "arcane_school.mentalism", result.ClassPackage.ArcaneSchool.Id );
        Assert.Null( result.ClassPackage.ArcaneThesis );
    }

    [Fact]
    public async Task GetCharacterById_LegacyClericWithoutDoctrine_ReturnsNullableDoctrinePackage()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 407 );
        DraftCharacter draftCharacter = DraftCharacter.Create(
            account.Id,
            "Legacy Kyra",
            AncestryType.Human );
        dbContext.Character.Add( draftCharacter );
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassId )
            .CurrentValue = "class.cleric";
        dbContext.Entry( draftCharacter )
            .Property( character => character.SelectedClassKeyAbility )
            .CurrentValue = AbilityType.Wisdom;
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharacterByIdHandler handler = CreateByIdHandler(
            dbContext,
            characterClassRepository: new CharacterClassRepository() );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.ClassPackage );
        Assert.Equal( "class.cleric", result.ClassPackage.ClassId );
        Assert.Null( result.ClassPackage.ClericDoctrine );
        Assert.Null( result.ClassPackage.Deity );
        Assert.Null( result.ClassPackage.ClericDomain );
        Assert.Null( result.ClassPackage.ClericSpellLoadout );
        Assert.Null( result.ClassPackage.ClericFocusPool );
        Assert.Contains(
            result.Proficiencies,
            proficiency => proficiency.TargetId == ProficiencyTargets.Fortitude.Id );
    }

    [Fact]
    public async Task GetCharacterById_UnbreakableGoblin_UsesHeritageHpOverride()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 403 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        CharacterBuilder builder = CreateBuilder(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository );
        builder.CreateCharacter( account.Id, "Fumbus", AncestryType.Goblin );
        builder.SetAncestryPackage( "goblin.unbreakable", "goblin.burn_it" );
        builder.ApplyFreeBoosts( [ AbilityType.Constitution ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Constitution );
        builder.SetClass( "class.fighter", AbilityType.Strength );
        builder.SetFinalFreeBoosts(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Intelligence
            ] );
        DraftCharacter draftCharacter = builder.Build();
        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharacterByIdHandler handler = CreateByIdHandler(
            dbContext,
            ancestryRepository,
            backgroundRepository,
            characterClassRepository );

        CharacterDto result = await handler.Handle(
            new GetCharacterByIdCommand( account.UserId, draftCharacter.Id ),
            CancellationToken.None );

        Assert.NotNull( result.AncestryPackage );
        Assert.Equal( 10, result.AncestryPackage.EffectiveBaseHitPoints );
        Assert.NotNull( result.DerivedStatistics );
        Assert.Equal( 23, result.DerivedStatistics.HitPoints.Maximum );
        Assert.Equal( 10, result.DerivedStatistics.HitPoints.Ancestry );
        Assert.Equal( 10, result.DerivedStatistics.HitPoints.Class );
        Assert.Equal( 3, result.DerivedStatistics.HitPoints.ConstitutionModifier );
    }

    [Fact]
    public async Task GetCharacters_WhenCharacterIsComplete_ReturnsDerivedStatistics()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 404 );
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        CharacterBuilder builder = CreateBuilder(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository );
        builder.CreateCharacter( account.Id, "Valeros", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Constitution ] );
        builder.SetBackground(
            "background.warrior",
            AbilityType.Strength,
            AbilityType.Constitution );
        builder.SetClass( "class.fighter", AbilityType.Strength );
        builder.SetFinalFreeBoosts(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom
            ] );
        dbContext.Character.Add( builder.Build() );
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        GetCharactersHandler handler = new GetCharactersHandler(
            new CharacterRepository( dbContext ),
            new CharacterDetailsDtoMapper(
                ancestryRepository,
                backgroundRepository,
                characterClassRepository,
                new SkillRepository() ) );

        IReadOnlyCollection<CharacterDto> result = await handler.Handle(
            new GetCharactersCommand( account.UserId ),
            CancellationToken.None );

        CharacterDto character = Assert.Single( result );
        Assert.NotNull( character.DerivedStatistics );
        Assert.Equal( 21, character.DerivedStatistics.HitPoints.Maximum );
        Assert.Equal( 6, character.DerivedStatistics.Perception.Total );
        Assert.Equal( AbilityType.Wisdom, character.DerivedStatistics.Perception.Ability );
        Assert.Equal( ProficiencyRank.Expert, character.DerivedStatistics.Perception.ProficiencyRank );
        Assert.Equal( 5, character.DerivedStatistics.Perception.ProficiencyBonus );
        Assert.Equal(
            [ "class.fighter.initial_proficiencies" ],
            character.DerivedStatistics.Perception.SourceGrantIds );
        Assert.Equal( 8, character.DerivedStatistics.SavingThrows.Fortitude.Total );
        Assert.Equal( AbilityType.Constitution, character.DerivedStatistics.SavingThrows.Fortitude.Ability );
        Assert.Equal( 6, character.DerivedStatistics.SavingThrows.Reflex.Total );
        Assert.Equal( AbilityType.Dexterity, character.DerivedStatistics.SavingThrows.Reflex.Ability );
        Assert.Equal( 4, character.DerivedStatistics.SavingThrows.Will.Total );
        Assert.Equal( AbilityType.Wisdom, character.DerivedStatistics.SavingThrows.Will.Ability );
        Assert.Equal( 16, character.DerivedStatistics.SkillModifiers.General.Count );
        CharacterProficiencyStatisticDto acrobatics = character.DerivedStatistics.SkillModifiers.General
            .Single( skill => skill.TargetId == "skill.acrobatics" );
        Assert.Equal( ProficiencyRank.Untrained, acrobatics.ProficiencyRank );
        Assert.Equal( AbilityType.Dexterity, acrobatics.Ability );
        Assert.Equal( 1, acrobatics.Total );
        CharacterProficiencyStatisticDto intimidation = character.DerivedStatistics.SkillModifiers.General
            .Single( skill => skill.TargetId == "skill.intimidation" );
        Assert.Equal( ProficiencyRank.Trained, intimidation.ProficiencyRank );
        Assert.Equal( AbilityType.Charisma, intimidation.Ability );
        Assert.Equal( 3, intimidation.Total );
        Assert.Equal(
            [ "background.warrior.skill" ],
            intimidation.SourceGrantIds );
        CharacterProficiencyStatisticDto warfareLore = Assert.Single(
            character.DerivedStatistics.SkillModifiers.Lore );
        Assert.Equal( "lore.warfare", warfareLore.TargetId );
        Assert.Equal( AbilityType.Intelligence, warfareLore.Ability );
        Assert.Equal( ProficiencyRank.Trained, warfareLore.ProficiencyRank );
        Assert.Equal( 3, warfareLore.Total );
        Assert.Equal( "skill.intimidation", Assert.Single( character.Training.Skills ).Id );
        Assert.Equal( "lore.warfare", Assert.Single( character.Training.Lore ).Id );
    }

    [Fact]
    public void Convert_WhenCharacterHasClassAndClassRepositoryIsMissing_Throws()
    {
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        CharacterBuilder builder = CreateBuilder(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository );
        builder.CreateCharacter( 1, "Valeros", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Constitution ] );
        builder.SetBackground(
            "background.warrior",
            AbilityType.Strength,
            AbilityType.Constitution );
        builder.SetClass( "class.fighter", AbilityType.Strength );
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper();

        Assert.Throws<InvalidOperationException>( () => mapper.Convert( builder.Build() ) );
    }

    [Fact]
    public void Convert_WhenCompleteCharacterAndSkillRepositoryIsMissing_Throws()
    {
        AncestryRepository ancestryRepository = new AncestryRepository();
        BackgroundRepository backgroundRepository = new BackgroundRepository();
        CharacterClassRepository characterClassRepository = new CharacterClassRepository();
        CharacterBuilder builder = CreateBuilder(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository );
        builder.CreateCharacter( 1, "Valeros", AncestryType.Human );
        builder.SetAncestryPackage( "human.skilled", "human.cooperative_nature" );
        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Constitution ] );
        builder.SetBackground(
            "background.warrior",
            AbilityType.Strength,
            AbilityType.Constitution );
        builder.SetClass( "class.fighter", AbilityType.Strength );
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository );

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>( () =>
            mapper.Convert( builder.Build() ) );

        Assert.Equal( "Skill repository is required to map skill modifiers.", exception.Message );
    }

    [Fact]
    public void Convert_IncludesStoredAvatarIdentifierAndResolvedPath()
    {
        DraftCharacter character = DraftCharacter.Create( 1, "Avatar Test", AncestryType.Human );
        CharacterDetailsDtoMapper mapper = new CharacterDetailsDtoMapper( avatarCatalog: new AvatarCatalog() );

        CharacterDto result = mapper.Convert( character );

        Assert.Equal( AvatarIds.Unknown.Value, result.AvatarId );
        Assert.Equal( AvatarCatalog.UnknownPath, result.AvatarPath );
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
        CharacterDetailsDtoMapper characterDetailsDtoMapper = new CharacterDetailsDtoMapper();

        return new GetCharactersHandler( characterRepository, characterDetailsDtoMapper );
    }

    private static GetCharacterByIdHandler CreateByIdHandler(
        CharacterManagementDbContext dbContext,
        AncestryRepository? ancestryRepository = null,
        BackgroundRepository? backgroundRepository = null,
        CharacterClassRepository? characterClassRepository = null )
    {
        CharacterRepository characterRepository = new CharacterRepository( dbContext );
        CharacterDetailsDtoMapper characterDetailsDtoMapper = new CharacterDetailsDtoMapper(
            ancestryRepository,
            backgroundRepository,
            characterClassRepository,
            new SkillRepository() );

        return new GetCharacterByIdHandler( characterRepository, characterDetailsDtoMapper );
    }

    private static CharacterBuilder CreateBuilder(
        AncestryRepository ancestryRepository,
        BackgroundRepository backgroundRepository,
        CharacterClassRepository characterClassRepository )
    {
        return new CharacterBuilder(
            ancestryRepository,
            backgroundRepository: backgroundRepository,
            characterClassRepository: characterClassRepository,
            skillRepository: new SkillRepository() );
    }

    private static string[] ClericCantripIds()
    {
        return [ "spell.daze", "spell.detect_magic", "spell.divine_lance", "spell.guidance", "spell.light" ];
    }

    private static string[] DruidCantripIds()
    {
        return [ "spell.caustic_blast", "spell.detect_magic", "spell.electric_arc", "spell.guidance", "spell.light" ];
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
