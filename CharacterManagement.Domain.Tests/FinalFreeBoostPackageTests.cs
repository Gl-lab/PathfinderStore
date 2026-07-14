using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class FinalFreeBoostPackageTests
{
    [Fact]
    public void SetFinalFreeBoosts_ValidPackage_AppliesFourBoosts()
    {
        DraftCharacter character = CreateCharacterWithClass();

        character.SetFinalFreeBoosts(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Wisdom
            ] );

        Assert.Equal( 4, character.AppliedFinalFreeBoosts.Count );
        Assert.Equal( 12, character.AbilityScores.Strength.Value );
        Assert.Equal( 16, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 14, character.AbilityScores.Constitution.Value );
        Assert.Equal( 14, character.AbilityScores.Wisdom.Value );
        Assert.True( character.HasFinalFreeBoostPackage );
    }

    [Fact]
    public void SetFinalFreeBoosts_ClassNotSet_ThrowsWithoutChangingCharacter()
    {
        DraftCharacter character = CreateCharacterWithoutClass();

        Assert.Throws<CharacterManagementException>( () =>
            character.SetFinalFreeBoosts(
                [
                    AbilityType.Strength,
                    AbilityType.Dexterity,
                    AbilityType.Constitution,
                    AbilityType.Intelligence
                ] ) );

        Assert.Empty( character.AppliedFinalFreeBoosts );
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Theory]
    [MemberData( nameof( InvalidPackageSizes ) )]
    public void SetFinalFreeBoosts_InvalidCount_Throws( IReadOnlyList<AbilityType> boosts )
    {
        DraftCharacter character = CreateCharacterWithClass();

        Assert.Throws<CharacterManagementException>( () =>
            character.SetFinalFreeBoosts( boosts ) );

        Assert.Empty( character.AppliedFinalFreeBoosts );
    }

    [Fact]
    public void SetFinalFreeBoosts_DuplicateAbility_ThrowsWithoutChangingCharacter()
    {
        DraftCharacter character = CreateCharacterWithClass();

        Assert.Throws<CharacterManagementException>( () =>
            character.SetFinalFreeBoosts(
                [
                    AbilityType.Strength,
                    AbilityType.Strength,
                    AbilityType.Constitution,
                    AbilityType.Intelligence
                ] ) );

        Assert.Empty( character.AppliedFinalFreeBoosts );
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void SetFinalFreeBoosts_AbilityBoostedByPreviousPackages_ReachesEighteen()
    {
        DraftCharacter character = CreateCharacterWithConstitutionAtSixteen();

        character.SetFinalFreeBoosts(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Intelligence
            ] );

        Assert.Equal( 18, character.AbilityScores.Constitution.Value );
    }

    [Fact]
    public void SetFinalFreeBoosts_CalledTwice_ReplacesOnlyFinalBoosts()
    {
        DraftCharacter character = CreateCharacterWithClass();
        character.SetFinalFreeBoosts(
            [
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Intelligence
            ] );

        character.SetFinalFreeBoosts(
            [
                AbilityType.Constitution,
                AbilityType.Intelligence,
                AbilityType.Wisdom,
                AbilityType.Charisma
            ] );

        Assert.Equal( 10, character.AbilityScores.Strength.Value );
        Assert.Equal( 14, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 14, character.AbilityScores.Constitution.Value );
        Assert.Equal( 14, character.AbilityScores.Intelligence.Value );
        Assert.Equal( 14, character.AbilityScores.Wisdom.Value );
        Assert.Equal( 12, character.AbilityScores.Charisma.Value );
        Assert.Equal(
            [
                AbilityType.Constitution,
                AbilityType.Intelligence,
                AbilityType.Wisdom,
                AbilityType.Charisma
            ],
            character.AppliedFinalFreeBoosts );
    }

    [Fact]
    public void SetFinalFreeBoosts_SamePackageTwice_DoesNotAccumulateBoosts()
    {
        DraftCharacter character = CreateCharacterWithClass();
        AbilityType[] boosts =
        [
            AbilityType.Strength,
            AbilityType.Dexterity,
            AbilityType.Constitution,
            AbilityType.Intelligence
        ];
        character.SetFinalFreeBoosts( boosts );

        character.SetFinalFreeBoosts( boosts );

        Assert.Equal( 12, character.AbilityScores.Strength.Value );
        Assert.Equal( 16, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 14, character.AbilityScores.Constitution.Value );
        Assert.Equal( 14, character.AbilityScores.Intelligence.Value );
    }

    [Fact]
    public void SetFinalFreeBoosts_CalledAfterClassTraining_RemovesStaleTraining()
    {
        DraftCharacter character = CreateCharacterWithoutClass();
        CharacterClass characterClass = CreateClassWithTraining();
        character.SetClassPackage( characterClass, AbilityType.Dexterity );
        AbilityType[] boosts =
        [
            AbilityType.Strength,
            AbilityType.Dexterity,
            AbilityType.Constitution,
            AbilityType.Charisma,
        ];
        character.SetFinalFreeBoosts( boosts );
        character.SetClassTraining(
            characterClass,
            [ new ClassSkillGrantChoice( "class.test.skill.arcana", null, null ) ],
            [ new ClassTrainingTargetChoice( "skill.athletics", null ) ],
            [
                new SkillDefinition(
                    "skill.arcana",
                    "Arcana",
                    AbilityType.Intelligence,
                    SourceReference.Unknown ),
                new SkillDefinition(
                    "skill.athletics",
                    "Athletics",
                    AbilityType.Strength,
                    SourceReference.Unknown ),
            ] );

        character.SetFinalFreeBoosts( boosts );

        Assert.Empty( character.TrainedSkills );
    }

    [Fact]
    public void SetFinalFreeBoosts_ReplacementWouldExceedCap_PreservesPreviousPackage()
    {
        DraftCharacter character = CreateCharacterWithClass();
        AbilityType[] initialBoosts =
        [
            AbilityType.Dexterity,
            AbilityType.Constitution,
            AbilityType.Intelligence,
            AbilityType.Wisdom
        ];
        character.SetFinalFreeBoosts( initialBoosts );
        character.UpdateAbilityScore( AbilityType.Strength, 18 );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetFinalFreeBoosts(
                [
                    AbilityType.Strength,
                    AbilityType.Dexterity,
                    AbilityType.Constitution,
                    AbilityType.Intelligence
                ] ) );

        Assert.Equal( initialBoosts, character.AppliedFinalFreeBoosts );
        Assert.Equal( 18, character.AbilityScores.Strength.Value );
        Assert.Equal( 16, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 14, character.AbilityScores.Wisdom.Value );
    }

    public static IEnumerable<object[]> InvalidPackageSizes()
    {
        yield return
        [
            new AbilityType[]
            {
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution
            }
        ];
        yield return
        [
            new AbilityType[]
            {
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Intelligence,
                AbilityType.Wisdom
            }
        ];
    }

    private static DraftCharacter CreateCharacterWithClass()
    {
        DraftCharacter character = CreateCharacterWithoutClass();
        CharacterClass fighter = CreateClass( AbilityType.Dexterity );
        character.SetClassPackage( fighter, AbilityType.Dexterity );
        return character;
    }

    private static DraftCharacter CreateCharacterWithConstitutionAtSixteen()
    {
        Ancestry ancestry = new Ancestry(
            AncestryType.Dwarf,
            [
                AncestryBoostSlot.Fixed( AbilityType.Constitution ),
                AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
                AncestryBoostSlot.Free()
            ],
            [ AbilityType.Charisma ],
            10,
            RaceSizeType.Medium,
            20 );
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Dwarf );
        character.SetAncestry( ancestry );
        character.SetFreeBoosts( [ AbilityType.Intelligence ] );
        character.SetBackgroundPackage(
            CreateBackground(),
            AbilityType.Constitution,
            AbilityType.Dexterity );
        character.SetClassPackage(
            CreateClass( AbilityType.Constitution ),
            AbilityType.Constitution );
        return character;
    }

    private static DraftCharacter CreateCharacterWithoutClass()
    {
        Ancestry ancestry = new Ancestry(
            AncestryType.Human,
            [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
            [],
            8,
            RaceSizeType.Medium,
            25 );
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Human );
        character.SetAncestry( ancestry );
        character.SetFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        character.SetBackgroundPackage(
            CreateBackground(),
            AbilityType.Dexterity,
            AbilityType.Constitution );
        return character;
    }

    private static Background CreateBackground()
    {
        return new Background(
            "background.acrobat",
            "Acrobat",
            new SourceReference( "Player Core", 60 ),
            [ AbilityType.Strength, AbilityType.Dexterity, AbilityType.Constitution ],
            1,
            [] );
    }

    private static CharacterClass CreateClass( AbilityType keyAbility )
    {
        return new CharacterClass(
            "class.test",
            "Test",
            new SourceReference( "Player Core", 1 ),
            8,
            [ keyAbility ],
            [
                new ProficiencyGrant(
                    ProficiencyTargets.Perception,
                    ProficiencyRank.Trained,
                    "class.test.initial_proficiencies" ),
            ],
            [],
            0,
            null,
            [],
            [] );
    }

    private static CharacterClass CreateClassWithTraining()
    {
        return new CharacterClass(
            "class.test",
            "Test",
            SourceReference.Unknown,
            8,
            [ AbilityType.Dexterity ],
            [
                new ProficiencyGrant(
                    ProficiencyTargets.Perception,
                    ProficiencyRank.Trained,
                    "class.test.initial_proficiencies" ),
            ],
            [ new ClassSkillGrantDescriptor( "class.test.skill.arcana", [ "skill.arcana" ] ) ],
            0,
            null,
            [],
            [] );
    }
}
