using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class AvatarIdTests
{
    [Fact]
    public void CreateCharacterWithoutAvatarUsesPermanentUnknownAvatar()
    {
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Human );

        Assert.Equal( AvatarIds.Unknown, character.AvatarId );
    }

    [Fact]
    public void CreateCharacterStoresProvidedAvatar()
    {
        AvatarId avatarId = AvatarId.Create( "avatar.human.fighter.male.01" );

        DraftCharacter character = DraftCharacter.Create(
            1,
            "Tester",
            AncestryType.Human,
            gender: CharacterGender.Male,
            avatarId: avatarId );

        Assert.Equal( avatarId, character.AvatarId );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( " " )]
    public void AvatarIdRejectsEmptyValues( string value )
    {
        Assert.Throws<CharacterManagementException>( () => AvatarId.Create( value ) );
    }
}
