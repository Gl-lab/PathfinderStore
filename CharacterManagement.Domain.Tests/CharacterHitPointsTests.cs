using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class CharacterHitPointsTests
{
    [Theory]
    [InlineData( 16, 3, 21 )]
    [InlineData( 10, 0, 18 )]
    [InlineData( 8, -1, 17 )]
    public void Calculate_BaseAncestryHp_UsesConstitutionModifier(
        int constitutionScore,
        int expectedModifier,
        int expectedMaximum )
    {
        Ancestry ancestry = CreateAncestry( 10 );
        CharacterClass characterClass = CreateClass( "class.druid", 8 );
        DraftCharacter character = CreateCharacter( ancestry, characterClass );
        character.UpdateAbilityScore( AbilityType.Constitution, constitutionScore );

        CharacterHitPoints result = CharacterHitPoints.Calculate(
            character,
            ancestry,
            characterClass );

        Assert.Equal( 10, result.AncestryHitPoints );
        Assert.Equal( 8, result.ClassHitPoints );
        Assert.Equal( expectedModifier, result.ConstitutionModifier );
        Assert.Equal( expectedMaximum, result.MaximumHitPoints );
    }

    [Fact]
    public void Calculate_SelectedHeritageOverridesAncestryHp()
    {
        Ancestry ancestry = CreateAncestry( 6, heritageHitPointsOverride: 10 );
        CharacterClass characterClass = CreateClass( "class.fighter", 10 );
        DraftCharacter character = CreateCharacter( ancestry, characterClass );
        character.UpdateAbilityScore( AbilityType.Constitution, 12 );

        CharacterHitPoints result = CharacterHitPoints.Calculate(
            character,
            ancestry,
            characterClass );

        Assert.Equal( 10, result.AncestryHitPoints );
        Assert.Equal( 21, result.MaximumHitPoints );
    }

    [Fact]
    public void GetEffectiveBaseHitPoints_ConflictingOverrides_Throws()
    {
        Ancestry ancestry = CreateAncestry(
            6,
            heritageHitPointsOverride: 10,
            featHitPointsOverride: 12 );

        Assert.Throws<CharacterManagementException>( () =>
            ancestry.GetEffectiveBaseHitPoints(
                "goblin.unbreakable",
                "goblin.test_feat" ) );
    }

    [Theory]
    [InlineData( 0 )]
    [InlineData( -1 )]
    public void GetEffectiveBaseHitPoints_NonPositiveOverride_Throws( int hitPointsOverride )
    {
        Ancestry ancestry = CreateAncestry(
            6,
            heritageHitPointsOverride: hitPointsOverride );

        Assert.Throws<CharacterManagementException>( () =>
            ancestry.GetEffectiveBaseHitPoints(
                "goblin.unbreakable",
                "goblin.test_feat" ) );
    }

    [Fact]
    public void Calculate_ConstitutionChanges_RecalculatesWithoutStoredHitPoints()
    {
        Ancestry ancestry = CreateAncestry( 10 );
        CharacterClass characterClass = CreateClass( "class.druid", 8 );
        DraftCharacter character = CreateCharacter( ancestry, characterClass );

        character.UpdateAbilityScore( AbilityType.Constitution, 10 );
        CharacterHitPoints initial = CharacterHitPoints.Calculate(
            character,
            ancestry,
            characterClass );

        character.UpdateAbilityScore( AbilityType.Constitution, 16 );
        CharacterHitPoints updated = CharacterHitPoints.Calculate(
            character,
            ancestry,
            characterClass );

        Assert.Equal( 18, initial.MaximumHitPoints );
        Assert.Equal( 21, updated.MaximumHitPoints );
    }

    [Fact]
    public void Calculate_ClassCatalogEntryDoesNotMatch_Throws()
    {
        Ancestry ancestry = CreateAncestry( 8 );
        CharacterClass selectedClass = CreateClass( "class.fighter", 10 );
        CharacterClass anotherClass = CreateClass( "class.wizard", 6 );
        DraftCharacter character = CreateCharacter( ancestry, selectedClass );

        Assert.Throws<CharacterManagementException>( () =>
            CharacterHitPoints.Calculate( character, ancestry, anotherClass ) );
    }

    [Fact]
    public void Calculate_AncestryCatalogEntryDoesNotMatch_Throws()
    {
        Ancestry ancestry = CreateAncestry( 8 );
        CharacterClass characterClass = CreateClass( "class.fighter", 10 );
        DraftCharacter character = CreateCharacter( ancestry, characterClass );
        Ancestry anotherAncestry = new Ancestry(
            AncestryType.Elf,
            [],
            [],
            6,
            RaceSizeType.Medium,
            30 );

        Assert.Throws<CharacterManagementException>( () =>
            CharacterHitPoints.Calculate( character, anotherAncestry, characterClass ) );
    }

    private static DraftCharacter CreateCharacter(
        Ancestry ancestry,
        CharacterClass characterClass )
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Tester",
            ancestry.AncestryType );
        character.SetAncestryPackage(
            null,
            ancestry,
            "goblin.unbreakable",
            "goblin.test_feat",
            new CommonAncestryChoiceAvailabilityPolicy() );
        character.SetFreeBoosts( [] );
        character.SetBackgroundPackage(
            CreateBackground(),
            AbilityType.Strength,
            AbilityType.Dexterity );
        character.SetClassPackage(
            characterClass,
            characterClass.KeyAbilityOptions.Single(),
            druidicOrder: characterClass.Id == "class.druid" ? CreateDruidicOrder() : null,
            druidSpellLoadout: characterClass.Id == "class.druid"
                ? DruidSpellTestData.CreateLoadout()
                : null );
        return character;
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

    private static Ancestry CreateAncestry(
        int baseHitPoints,
        int? heritageHitPointsOverride = null,
        int? featHitPointsOverride = null )
    {
        Heritage heritage = new Heritage(
            "goblin.unbreakable",
            AncestryType.Goblin,
            "Unbreakable Goblin",
            SourceReference.Unknown,
            AncestryChoiceRarity.Common,
            [],
            [],
            [
                new AncestryEffectDescriptor(
                    "goblin.unbreakable.effect",
                    heritageHitPointsOverride.HasValue
                        ? AncestryEffectKind.BaseHpOverride
                        : AncestryEffectKind.RuleEffect,
                    "Heritage effect.",
                    BaseHitPointsOverride: heritageHitPointsOverride )
            ],
            [] );
        AncestryFeat ancestryFeat = new AncestryFeat(
            "goblin.test_feat",
            AncestryType.Goblin,
            "Test Feat",
            SourceReference.Unknown,
            1,
            AncestryChoiceRarity.Common,
            [],
            [],
            [],
            [
                new AncestryEffectDescriptor(
                    "goblin.test_feat.effect",
                    featHitPointsOverride.HasValue
                        ? AncestryEffectKind.BaseHpOverride
                        : AncestryEffectKind.RuleEffect,
                    "Feat effect.",
                    BaseHitPointsOverride: featHitPointsOverride )
            ],
            [] );

        return new Ancestry(
            AncestryType.Goblin,
            [],
            [],
            baseHitPoints,
            RaceSizeType.Small,
            25,
            heritages: [ heritage ],
            ancestryFeats: [ ancestryFeat ] );
    }

    private static Background CreateBackground()
    {
        return new Background(
            "background.test",
            "Test",
            SourceReference.Unknown,
            [ AbilityType.Strength ],
            1,
            [] );
    }

    private static CharacterClass CreateClass( string id, int baseHitPoints )
    {
        return new CharacterClass(
            id,
            id,
            SourceReference.Unknown,
            baseHitPoints,
            [ AbilityType.Intelligence ],
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
}
