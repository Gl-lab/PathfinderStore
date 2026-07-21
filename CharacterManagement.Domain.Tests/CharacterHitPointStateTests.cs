using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Tests;

public sealed class CharacterHitPointStateTests
{
    [Fact]
    public void Commands_CompletedCharacter_ApplyTemporaryAbsorptionAndBounds()
    {
        DraftCharacter character = DraftCharacter.Create( 1, "Valeros", AncestryType.Human );
        character.FinalizeCreation( DateTimeOffset.UtcNow );

        character.GrantTemporaryHitPoints( 5, 20 );
        character.ApplyDamage( 8, 20 );
        Assert.Equal( 17, character.GetHitPointState( 20 ).Current );
        Assert.Equal( 0, character.GetHitPointState( 20 ).Temporary );

        character.RestoreHitPoints( 99, 20 );
        Assert.Equal( 20, character.GetHitPointState( 20 ).Current );

        character.ApplyDamage( 99, 20 );
        Assert.Equal( 0, character.GetHitPointState( 20 ).Current );
    }

    [Fact]
    public void GetHitPointState_MaximumDecreases_ClampsStoredCurrentHitPoints()
    {
        DraftCharacter character = DraftCharacter.Create( 1, "Valeros", AncestryType.Human );
        character.FinalizeCreation( DateTimeOffset.UtcNow );
        character.RestoreHitPoints( 1, 20 );

        Assert.Equal( 12, character.GetHitPointState( 12 ).Current );
    }

    [Fact]
    public void Commands_DraftCharacter_Throw()
    {
        DraftCharacter character = DraftCharacter.Create( 1, "Valeros", AncestryType.Human );

        Assert.Throws<CharacterManagementException>( () => character.ApplyDamage( 1, 20 ) );
    }
}
