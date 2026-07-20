using FluentValidation;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class SetCharacterGenderCommandValidatorTests
{
    [Theory]
    [InlineData( CharacterGender.Male )]
    [InlineData( CharacterGender.Female )]
    public void Validate_WithSelectableGender_DoesNotThrow( CharacterGender gender )
    {
        SetCharacterGenderCommandValidator validator = new SetCharacterGenderCommandValidator();

        validator.ValidateAndThrow( new SetCharacterGenderCommand( 1, 2, gender ) );
    }

    [Theory]
    [InlineData( CharacterGender.NotSpecified )]
    [InlineData( ( CharacterGender )99 )]
    public void Validate_WithNonSelectableGender_ThrowsValidationException(
        CharacterGender gender )
    {
        SetCharacterGenderCommandValidator validator = new SetCharacterGenderCommandValidator();

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new SetCharacterGenderCommand( 1, 2, gender ) ) );
    }

    [Theory]
    [InlineData( 0, 2 )]
    [InlineData( 1, 0 )]
    public void Validate_WithNonPositiveIdentifier_ThrowsValidationException(
        int userId,
        int characterId )
    {
        SetCharacterGenderCommandValidator validator = new SetCharacterGenderCommandValidator();

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new SetCharacterGenderCommand(
                userId,
                characterId,
                CharacterGender.Male ) ) );
    }
}
