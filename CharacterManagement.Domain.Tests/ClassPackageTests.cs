using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class ClassPackageTests
{
    [Fact]
    public void SetClassPackage_ValidChoice_AppliesClassBoost()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass(
            "class.fighter",
            AbilityType.Strength,
            AbilityType.Dexterity );

        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Equal( fighter.Id, character.SelectedClassId );
        Assert.Equal( AbilityType.Strength, character.SelectedClassKeyAbility );
        Assert.Equal( 12, character.AbilityScores.Strength.Value );
        Assert.True( character.HasClassBoostPackage );
    }

    [Fact]
    public void SetClassPackage_BackgroundNotSet_ThrowsWithoutChangingCharacter()
    {
        DraftCharacter character = CreateCharacterWithoutBackground();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( fighter, AbilityType.Strength ) );

        Assert.Null( character.SelectedClassId );
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void SetClassPackage_KeyAbilityNotAllowed_ThrowsWithoutChangingCharacter()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass bard = CreateClass( "class.bard", AbilityType.Charisma );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage(
                bard,
                AbilityType.Strength,
                bardMuse: CreateBardMuse() ) );

        Assert.Null( character.SelectedClassId );
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void SetClassPackage_KeyAbilityAlreadyBoostedByPreviousPackages_IsAllowed()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateBackground();
        CharacterClass wizard = CreateClass( "class.wizard", AbilityType.Intelligence );
        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence );

        character.SetClassPackage( wizard, AbilityType.Intelligence );

        Assert.Equal( 16, character.AbilityScores.Intelligence.Value );
    }

    [Fact]
    public void SetClassPackage_CalledTwice_ReplacesOnlyClassBoost()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateBackground();
        CharacterClass wizard = CreateClass( "class.wizard", AbilityType.Intelligence );
        CharacterClass bard = CreateClass( "class.bard", AbilityType.Charisma );
        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence );
        character.SetClassPackage( wizard, AbilityType.Intelligence );

        character.SetClassPackage(
            bard,
            AbilityType.Charisma,
            bardMuse: CreateBardMuse() );

        Assert.Equal( bard.Id, character.SelectedClassId );
        Assert.Equal( 14, character.AbilityScores.Intelligence.Value );
        Assert.Equal( 12, character.AbilityScores.Charisma.Value );
        Assert.Equal( 12, character.AbilityScores.Dexterity.Value );
    }

    [Fact]
    public void CharacterBuilder_SetClass_UsesCatalogEntry()
    {
        Ancestry ancestry = CreateAncestry();
        CharacterClass fighter = CreateClass(
            "class.fighter",
            AbilityType.Strength,
            AbilityType.Dexterity );
        CharacterBuilder builder = new CharacterBuilder(
            new StubAncestryRepository( ancestry ),
            backgroundRepository: new StubBackgroundRepository( CreateBackground() ),
            characterClassRepository: new StubCharacterClassRepository( fighter ) );
        builder.CreateCharacter( 1, "Tester", AncestryType.Human );
        builder.SetAncestry( AncestryType.Human );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Constitution );

        builder.SetClass( fighter.Id, AbilityType.Dexterity );

        DraftCharacter character = builder.Build();
        Assert.Equal( fighter.Id, character.SelectedClassId );
        Assert.Equal( 14, character.AbilityScores.Dexterity.Value );
    }

    [Fact]
    public void CharacterBuilder_SetClass_ResolvesClericDoctrineCatalogEntry()
    {
        Ancestry ancestry = CreateAncestry();
        CharacterClass cleric = CreateClass( "class.cleric", AbilityType.Wisdom );
        ClericDoctrine doctrine = CreateDoctrine( "cloistered" );
        Deity deity = CreateDeity();
        CharacterBuilder builder = new CharacterBuilder(
            new StubAncestryRepository( ancestry ),
            backgroundRepository: new StubBackgroundRepository( CreateBackground() ),
            characterClassRepository: new StubCharacterClassRepository( cleric ),
            skillRepository: new StubSkillRepository( CreateSkills() ),
            clericDoctrineRepository: new StubClericDoctrineRepository( doctrine ),
            deityRepository: new StubDeityRepository( deity ) );
        builder.CreateCharacter( 1, "Tester", AncestryType.Human );
        builder.SetAncestry( AncestryType.Human );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Constitution );

        builder.SetClass(
            cleric.Id,
            AbilityType.Wisdom,
            clericDoctrineId: doctrine.Id,
            deityId: deity.Id,
            divineFont: DivineFont.Heal );

        Assert.Equal( doctrine.Id, builder.Build().SelectedClericDoctrineId );
    }

    [Fact]
    public void SetClassPackage_Rogue_AppliesRacketKeyAbilityAndTraining()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass rogue = CreateClass( "class.rogue", AbilityType.Dexterity );
        RogueRacket ruffian = CreateRacket(
            "ruffian",
            AbilityType.Strength,
            new RogueSkillGrantDescriptor(
                "rogue_racket.ruffian.skill.intimidation",
                "skill.intimidation",
                [] ) );

        character.SetClassPackage(
            rogue,
            AbilityType.Strength,
            ruffian,
            [],
            CreateSkills() );

        Assert.Equal( ruffian.Id, character.SelectedRogueRacketId );
        Assert.Equal( 12, character.AbilityScores.Strength.Value );
        Assert.Contains( character.TrainedSkills, skill => skill.SkillId == "skill.stealth" );
        Assert.Contains( character.TrainedSkills, skill => skill.SkillId == "skill.intimidation" );
    }

    [Fact]
    public void SetClassPackage_RogueWithoutRacket_AndNonRogueWithRacket_Throw()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass rogue = CreateClass( "class.rogue", AbilityType.Dexterity );
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        RogueRacket thief = CreateRacket(
            "thief",
            null,
            new RogueSkillGrantDescriptor(
                "rogue_racket.thief.skill.thievery",
                "skill.thievery",
                [] ) );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( rogue, AbilityType.Dexterity ) );
        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( fighter, AbilityType.Strength, thief ) );
    }

    [Fact]
    public void SetClassPackage_ReplacingRogueWithFighter_RemovesOnlyRogueTraining()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass rogue = CreateClass( "class.rogue", AbilityType.Dexterity );
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        RogueRacket thief = CreateRacket(
            "thief",
            null,
            new RogueSkillGrantDescriptor(
                "rogue_racket.thief.skill.thievery",
                "skill.thievery",
                [] ) );
        character.SetClassPackage(
            rogue,
            AbilityType.Dexterity,
            thief,
            [],
            CreateSkills() );

        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Null( character.SelectedRogueRacketId );
        Assert.DoesNotContain( character.TrainedSkills, skill =>
            skill.SourceGrantId.StartsWith( "class.rogue.", StringComparison.Ordinal ) ||
            skill.SourceGrantId.StartsWith( "rogue_racket.", StringComparison.Ordinal ) );
    }

    [Fact]
    public void SetClassPackage_ClericRequiresDoctrineAndStoresSelection()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass cleric = CreateClass( "class.cleric", AbilityType.Wisdom );
        ClericDoctrine doctrine = CreateDoctrine( "warpriest" );
        Deity deity = CreateDeity();

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( cleric, AbilityType.Wisdom ) );

        character.SetClassPackage(
            cleric,
            AbilityType.Wisdom,
            skillCatalog: CreateSkills(),
            clericDoctrine: doctrine,
            deity: deity,
            divineFont: DivineFont.Heal );

        Assert.Equal( doctrine.Id, character.SelectedClericDoctrineId );
        Assert.Equal( 14, character.AbilityScores.Wisdom.Value );
    }

    [Fact]
    public void SetClassPackage_NonClericWithDoctrine_ThrowsWithoutReplacingExistingPackage()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass cleric = CreateClass( "class.cleric", AbilityType.Wisdom );
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        ClericDoctrine doctrine = CreateDoctrine( "cloistered" );
        Deity deity = CreateDeity();
        character.SetClassPackage(
            cleric,
            AbilityType.Wisdom,
            skillCatalog: CreateSkills(),
            clericDoctrine: doctrine,
            deity: deity,
            divineFont: DivineFont.Heal );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage(
                fighter,
                AbilityType.Strength,
                clericDoctrine: doctrine ) );

        Assert.Equal( cleric.Id, character.SelectedClassId );
        Assert.Equal( doctrine.Id, character.SelectedClericDoctrineId );
        Assert.Equal( 14, character.AbilityScores.Wisdom.Value );
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void SetClassPackage_ReplacingClericWithFighter_ClearsDoctrine()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass cleric = CreateClass( "class.cleric", AbilityType.Wisdom );
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        character.SetClassPackage(
            cleric,
            AbilityType.Wisdom,
            skillCatalog: CreateSkills(),
            clericDoctrine: CreateDoctrine( "warpriest" ),
            deity: CreateDeity(),
            divineFont: DivineFont.Heal );

        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Null( character.SelectedClericDoctrineId );
        Assert.Equal( 12, character.AbilityScores.Wisdom.Value );
        Assert.Equal( 12, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void SetClassPackage_RangerRequiresHuntersEdgeAndInvalidCallIsAtomic()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        CharacterClass ranger = CreateClass( "class.ranger", AbilityType.Dexterity );
        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( ranger, AbilityType.Dexterity ) );

        Assert.Equal( "class.fighter", character.SelectedClassId );
        Assert.Null( character.SelectedHuntersEdgeId );
    }

    [Fact]
    public void SetClassPackage_RangerStoresEdgeAndChangingClassClearsIt()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass ranger = CreateClass( "class.ranger", AbilityType.Dexterity );
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        HuntersEdge huntersEdge = CreateHuntersEdge();
        character.SetClassPackage(
            ranger,
            AbilityType.Dexterity,
            huntersEdge: huntersEdge );

        Assert.Equal( huntersEdge.Id, character.SelectedHuntersEdgeId );

        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Null( character.SelectedHuntersEdgeId );
    }

    [Fact]
    public void SetClassPackage_NonRangerWithHuntersEdge_Throws()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage(
                fighter,
                AbilityType.Strength,
                huntersEdge: CreateHuntersEdge() ) );
    }

    [Fact]
    public void SetClassPackage_DruidRequiresOrderAndInvalidCallIsAtomic()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        CharacterClass druid = CreateClass( "class.druid", AbilityType.Wisdom );
        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( druid, AbilityType.Wisdom ) );

        Assert.Equal( "class.fighter", character.SelectedClassId );
        Assert.Null( character.SelectedDruidicOrderId );
    }

    [Fact]
    public void SetClassPackage_DruidStoresOrderAndChangingClassClearsIt()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass druid = CreateClass( "class.druid", AbilityType.Wisdom );
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        DruidicOrder druidicOrder = CreateDruidicOrder();
        character.SetClassPackage(
            druid,
            AbilityType.Wisdom,
            druidicOrder: druidicOrder );

        Assert.Equal( druidicOrder.Id, character.SelectedDruidicOrderId );

        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Null( character.SelectedDruidicOrderId );
    }

    [Fact]
    public void SetClassPackage_NonDruidWithOrder_Throws()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage(
                fighter,
                AbilityType.Strength,
                druidicOrder: CreateDruidicOrder() ) );
    }

    [Fact]
    public void SetClassPackage_BardRequiresMuseAndInvalidCallIsAtomic()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        CharacterClass bard = CreateClass( "class.bard", AbilityType.Charisma );
        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( bard, AbilityType.Charisma ) );

        Assert.Equal( "class.fighter", character.SelectedClassId );
        Assert.Null( character.SelectedBardMuseId );
    }

    [Fact]
    public void SetClassPackage_BardStoresMuseAndChangingClassClearsIt()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass bard = CreateClass( "class.bard", AbilityType.Charisma );
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        BardMuse bardMuse = CreateBardMuse();
        character.SetClassPackage(
            bard,
            AbilityType.Charisma,
            bardMuse: bardMuse );

        Assert.Equal( bardMuse.Id, character.SelectedBardMuseId );

        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Null( character.SelectedBardMuseId );
    }

    [Fact]
    public void SetClassPackage_NonBardWithMuse_Throws()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage(
                fighter,
                AbilityType.Strength,
                bardMuse: CreateBardMuse() ) );
    }

    [Fact]
    public void SetBackgroundPackage_AfterClassPackage_ThrowsWithoutRemovingClassEffects()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );
        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () => character.SetBackgroundPackage(
            CreateBackground(),
            AbilityType.Dexterity,
            AbilityType.Intelligence ) );

        Assert.Equal( fighter.Id, character.SelectedClassId );
        Assert.Equal( 12, character.AbilityScores.Strength.Value );
    }

    private static DraftCharacter CreateCharacter()
    {
        DraftCharacter character = CreateCharacterWithoutBackground();
        character.SetBackgroundPackage(
            CreateBackground(),
            AbilityType.Dexterity,
            AbilityType.Constitution );
        return character;
    }

    private static DraftCharacter CreateCharacterWithoutBackground()
    {
        Ancestry ancestry = CreateAncestry();
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Human );
        character.SetAncestry( ancestry );
        character.SetFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        return character;
    }

    private static Ancestry CreateAncestry()
    {
        return new Ancestry(
            AncestryType.Human,
            [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
            [],
            8,
            RaceSizeType.Medium,
            25 );
    }

    private static Background CreateBackground()
    {
        return new Background(
            "background.acrobat",
            "Acrobat",
            new SourceReference( "Player Core", 60 ),
            [ AbilityType.Strength, AbilityType.Dexterity ],
            1,
            [] );
    }

    private static CharacterClass CreateClass( string id, params AbilityType[] keyAbilities )
    {
        return new CharacterClass(
            id,
            id,
            new SourceReference( "Player Core", 1 ),
            8,
            keyAbilities,
            [
                new ProficiencyGrant(
                    ProficiencyTargets.Perception,
                    ProficiencyRank.Trained,
                    $"{id}.initial_proficiencies" ),
            ],
            [],
            0,
            null,
            [],
            [] );
    }

    private static RogueRacket CreateRacket(
        string id,
        AbilityType? alternativeKeyAbility,
        params RogueSkillGrantDescriptor[] skillGrants )
    {
        return new RogueRacket(
            $"rogue_racket.{id}",
            id,
            new SourceReference( "Player Core", 166 ),
            alternativeKeyAbility,
            skillGrants,
            [],
            [],
            [] );
    }

    private static HuntersEdge CreateHuntersEdge()
    {
        return new HuntersEdge(
            "hunters_edge.precision",
            "Precision",
            SourceReference.Unknown,
            [
                new HuntersEdgeEffectDescriptor(
                    "hunters_edge.precision.effect.level_1",
                    HuntersEdgeEffectKind.PrecisionDamage,
                    "Precision",
                    "Additional precision damage." ),
            ],
            [ CharacterClassDependencyType.ClassFeatureRules ] );
    }

    private static DruidicOrder CreateDruidicOrder()
    {
        return new DruidicOrder(
            "druidic_order.animal",
            "Animal",
            SourceReference.Unknown,
            new ClassSkillGrantDescriptor(
                "druidic_order.animal.skill.order",
                [ "skill.athletics" ] ),
            [
                new DruidicOrderBenefitDescriptor(
                    "feat.animal_companion",
                    DruidicOrderBenefitKind.ClassFeat,
                    "Animal Companion",
                    [] ),
                new DruidicOrderBenefitDescriptor(
                    "spell.heal_animal",
                    DruidicOrderBenefitKind.FocusSpell,
                    "Heal Animal",
                    [] ),
            ] );
    }

    private static BardMuse CreateBardMuse()
    {
        return new BardMuse(
            "bard_muse.enigma",
            "Enigma",
            SourceReference.Unknown,
            [
                new BardMuseBenefitDescriptor(
                    "feat.bardic_lore",
                    BardMuseBenefitKind.ClassFeat,
                    "Bardic Lore",
                    [] ),
                new BardMuseBenefitDescriptor(
                    "spell.sure_strike",
                    BardMuseBenefitKind.RepertoireSpell,
                    "Sure Strike",
                    [] ),
            ] );
    }

    private static ClericDoctrine CreateDoctrine( string id )
    {
        return new ClericDoctrine(
            $"cleric_doctrine.{id}",
            id,
            new SourceReference( "Player Core", 112 ),
            [],
            [],
            [] );
    }

    private static Deity CreateDeity()
    {
        return new Deity(
            "deity.test",
            "Test Deity",
            new SourceReference( "Player Core", 35 ),
            true,
            "skill.intimidation",
            [ new DeityFavoredWeapon( "weapon.longsword", "Longsword", FavoredWeaponCategory.Martial ) ],
            [ DivineFont.Heal ],
            [],
            null,
            [ "domain.truth" ],
            [] );
    }

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        SourceReference source = new SourceReference( "Player Core", 227 );
        return
        [
            new SkillDefinition( "skill.intimidation", "Intimidation", AbilityType.Charisma, source ),
            new SkillDefinition( "skill.stealth", "Stealth", AbilityType.Dexterity, source ),
            new SkillDefinition( "skill.thievery", "Thievery", AbilityType.Dexterity, source ),
        ];
    }

    private sealed class StubAncestryRepository : IAncestryRepository
    {
        private readonly Ancestry _ancestry;

        public StubAncestryRepository( Ancestry ancestry )
        {
            _ancestry = ancestry;
        }

        public IReadOnlyCollection<Ancestry> GetAll() => [ _ancestry ];

        public Ancestry GetAncestry( AncestryType ancestryType ) => _ancestry;
    }

    private sealed class StubCharacterClassRepository : ICharacterClassRepository
    {
        private readonly CharacterClass _characterClass;

        public StubCharacterClassRepository( CharacterClass characterClass )
        {
            _characterClass = characterClass;
        }

        public IReadOnlyCollection<CharacterClass> GetAll() => [ _characterClass ];

        public CharacterClass GetCharacterClass( string characterClassId ) => _characterClass;
    }

    private sealed class StubBackgroundRepository : IBackgroundRepository
    {
        private readonly Background _background;

        public StubBackgroundRepository( Background background )
        {
            _background = background;
        }

        public IReadOnlyCollection<Background> GetAll() => [ _background ];

        public Background GetBackground( string backgroundId ) => _background;
    }

    private sealed class StubClericDoctrineRepository : IClericDoctrineRepository
    {
        private readonly ClericDoctrine _doctrine;

        public StubClericDoctrineRepository( ClericDoctrine doctrine )
        {
            _doctrine = doctrine;
        }

        public IReadOnlyCollection<ClericDoctrine> GetAll() => [ _doctrine ];

        public ClericDoctrine GetClericDoctrine( string clericDoctrineId ) => _doctrine;
    }

    private sealed class StubDeityRepository : IDeityRepository
    {
        private readonly Deity _deity;

        public StubDeityRepository( Deity deity )
        {
            _deity = deity;
        }

        public IReadOnlyCollection<Deity> GetAll() => [ _deity ];

        public Deity GetDeity( string deityId ) => _deity;
    }

    private sealed class StubSkillRepository : ISkillRepository
    {
        private readonly IReadOnlyCollection<SkillDefinition> _skills;

        public StubSkillRepository( IReadOnlyCollection<SkillDefinition> skills )
        {
            _skills = skills;
        }

        public IReadOnlyCollection<SkillDefinition> GetAll() => _skills;

        public SkillDefinition GetSkill( string skillId ) => _skills.Single( skill => skill.Id == skillId );
    }
}
