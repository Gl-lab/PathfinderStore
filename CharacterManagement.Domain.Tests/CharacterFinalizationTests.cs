using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class CharacterFinalizationTests
{
    [Fact]
    public void FinalizeCreation_UsesUtcTimestampAndIsIdempotent()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: CharacterGender.Male );
        DateTimeOffset completedAtUtc = new DateTimeOffset( 2026, 7, 20, 21, 30, 0, TimeSpan.Zero );

        character.FinalizeCreation( completedAtUtc );
        character.FinalizeCreation( completedAtUtc.AddHours( 1 ) );

        Assert.Equal( CharacterCreationStatus.Completed, character.CreationStatus );
        Assert.Equal( completedAtUtc, character.CompletedAtUtc );
    }

    [Fact]
    public void FinalizeCreation_NonUtcTimestamp_Throws()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: CharacterGender.Male );
        DateTimeOffset nonUtc = new DateTimeOffset( 2026, 7, 21, 0, 30, 0, TimeSpan.FromHours( 3 ) );

        Assert.Throws<CharacterManagementException>( () => character.FinalizeCreation( nonUtc ) );
    }

    [Fact]
    public void CompletedCharacter_BuildMutations_Throw()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: CharacterGender.Male );
        character.FinalizeCreation( DateTimeOffset.UtcNow );

        Assert.Throws<CharacterManagementException>( () => character.Rename( "Changed" ) );
        Assert.Throws<CharacterManagementException>( () =>
            character.UpdateAbilityScore( AbilityType.Strength, 12 ) );
    }
}
