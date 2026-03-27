using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public class SetAncestryTests
{
    private static DraftCharacter CreateCharacter() =>
        DraftCharacter.Create( accountId: 1, name: "Thorin", raceId: 1 );

    private static Ancestry HumanAncestry() => new Ancestry(
        AncestryType.Human,
        abilityBoosts: [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
        abilityFlaws: [],
        baseHitPoints: 8,
        size: RaceSizeType.Medium,
        baseSpeed: 25 );

    private static Ancestry DwarfAncestry() => new Ancestry(
        AncestryType.Dwarf,
        abilityBoosts:
        [
            AncestryBoostSlot.Fixed( AbilityType.Constitution ),
            AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
            AncestryBoostSlot.Free()
        ],
        abilityFlaws: [ AbilityType.Charisma ],
        baseHitPoints: 10,
        size: RaceSizeType.Medium,
        baseSpeed: 20 );

    private static Ancestry GnomeAncestry() => new Ancestry(
        AncestryType.Gnome,
        abilityBoosts:
        [
            AncestryBoostSlot.Fixed( AbilityType.Constitution ),
            AncestryBoostSlot.Fixed( AbilityType.Charisma ),
            AncestryBoostSlot.Free()
        ],
        abilityFlaws: [ AbilityType.Strength ],
        baseHitPoints: 8,
        size: RaceSizeType.Small,
        baseSpeed: 25 );

    [Fact]
    public void SetAncestry_FixedBoost_IncreasesCharacteristic()
    {
        DraftCharacter character = CreateCharacter();

        character.SetAncestry( DwarfAncestry() );

        Assert.Equal( 12, character.AbilityScores.Constitution.Value );
        Assert.Equal( 12, character.AbilityScores.Wisdom.Value );
    }

    [Fact]
    public void SetAncestry_FreeBoost_DoesNotChangeCharacteristic()
    {
        DraftCharacter character = CreateCharacter();

        character.SetAncestry( HumanAncestry() );

        Assert.Equal( 10, character.AbilityScores.Strength.Value );
        Assert.Equal( 10, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 10, character.AbilityScores.Constitution.Value );
        Assert.Equal( 10, character.AbilityScores.Intelligence.Value );
        Assert.Equal( 10, character.AbilityScores.Wisdom.Value );
        Assert.Equal( 10, character.AbilityScores.Charisma.Value );
    }

    [Fact]
    public void SetAncestry_Flaw_DecreasesCharacteristic()
    {
        DraftCharacter character = CreateCharacter();

        character.SetAncestry( DwarfAncestry() );

        Assert.Equal( 8, character.AbilityScores.Charisma.Value );
    }

    [Fact]
    public void SetAncestry_Gnome_FixedAndFreeAndFlaw()
    {
        DraftCharacter character = CreateCharacter();

        character.SetAncestry( GnomeAncestry() );

        Assert.Equal( 8, character.AbilityScores.Strength.Value );
        Assert.Equal( 12, character.AbilityScores.Constitution.Value );
        Assert.Equal( 12, character.AbilityScores.Charisma.Value );
        Assert.Equal( 10, character.AbilityScores.Dexterity.Value ); // free — не применён
    }

    [Fact]
    public void SetAncestry_NullAncestry_Throws()
    {
        DraftCharacter character = CreateCharacter();

        Assert.Throws<ArgumentNullException>( () => character.SetAncestry( null! ) );
    }

    [Fact]
    public void SetAncestry_CalledTwice_PreviousBoostsRolledBack()
    {
        DraftCharacter character = CreateCharacter();

        character.SetAncestry( DwarfAncestry() ); // +Con, +Wis, -Cha
        character.SetAncestry( HumanAncestry() ); // нет фиксированных бустов

        Assert.Equal( 10, character.AbilityScores.Constitution.Value );
        Assert.Equal( 10, character.AbilityScores.Wisdom.Value );
        Assert.Equal( 10, character.AbilityScores.Charisma.Value );
    }

    [Fact]
    public void SetAncestry_CalledTwice_AbilityScoresReturnToBase()
    {
        DraftCharacter character = CreateCharacter();
        character.SetAncestry( DwarfAncestry() );        // +Con, +Wis, -Cha
        character.SetFreeBoosts( [ AbilityType.Strength ] ); // +Str

        character.SetAncestry( HumanAncestry() );

        Assert.Equal( 10, character.AbilityScores.Strength.Value );
        Assert.Equal( 10, character.AbilityScores.Constitution.Value );
        Assert.Equal( 10, character.AbilityScores.Wisdom.Value );
        Assert.Equal( 10, character.AbilityScores.Charisma.Value );
    }

[Fact]
    public void SetAncestry_CalledTwice_FreeBoostsReset()
    {
        DraftCharacter character = CreateCharacter();
        character.SetAncestry( HumanAncestry() );
        character.SetFreeBoosts( [ AbilityType.Strength, AbilityType.Intelligence ] );

        character.SetAncestry( DwarfAncestry() );

        Assert.Empty( character.AppliedFreeBoosts );
    }
}
