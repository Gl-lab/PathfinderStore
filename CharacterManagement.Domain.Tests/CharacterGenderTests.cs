using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class CharacterGenderTests
{
    [Fact]
    public void Create_WithoutGender_UsesLegacyNotSpecifiedState()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human );

        Assert.Equal( CharacterGender.NotSpecified, character.Gender );
    }

    [Theory]
    [InlineData( CharacterGender.Male )]
    [InlineData( CharacterGender.Female )]
    public void Create_WithSelectableGender_StoresGender( CharacterGender gender )
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: gender );

        Assert.Equal( gender, character.Gender );
    }

    [Fact]
    public void Create_WithUnknownGender_ThrowsDomainException()
    {
        Assert.Throws<CharacterManagementException>( () => DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: ( CharacterGender )99 ) );
    }

    [Theory]
    [InlineData( CharacterGender.Male )]
    [InlineData( CharacterGender.Female )]
    public void SetGender_WhenNotSpecified_SetsGender( CharacterGender gender )
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human );

        character.SetGender( gender );

        Assert.Equal( gender, character.Gender );
    }

    [Fact]
    public void SetGender_WhenAlreadySpecified_ThrowsDomainException()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: CharacterGender.Male );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetGender( CharacterGender.Female ) );
    }

    [Theory]
    [InlineData( CharacterGender.NotSpecified )]
    [InlineData( ( CharacterGender )99 )]
    public void SetGender_WithNonSelectableGender_ThrowsDomainException(
        CharacterGender gender )
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human );

        Assert.Throws<CharacterManagementException>( () => character.SetGender( gender ) );
    }
}
