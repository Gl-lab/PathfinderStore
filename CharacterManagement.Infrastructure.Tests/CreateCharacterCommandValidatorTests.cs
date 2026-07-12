using FluentValidation;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class CreateCharacterCommandValidatorTests
{
    [Fact]
    public void Validate_WhenUserIdIsNotPositive_ThrowsValidationException()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            0,
            new CreateCharacterRequestDto
            {
                Name = "Thorin",
                AncestryType = AncestryType.Human,
                HeritageId = "human.skilled",
                AncestryFeatId = "human.cooperative_nature",
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Fact]
    public void Validate_WhenCharacterNameIsEmpty_ThrowsValidationException()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            42,
            new CreateCharacterRequestDto
            {
                Name = "",
                AncestryType = AncestryType.Human,
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Fact]
    public void Validate_WhenCommandIsValid_DoesNotThrow()
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterCommand command = new CreateCharacterCommand(
            42,
            new CreateCharacterRequestDto
            {
                Name = "Thorin",
                AncestryType = AncestryType.Human,
                HeritageId = "human.skilled",
                AncestryFeatId = "human.cooperative_nature",
                FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
            } );

        validator.ValidateAndThrow( command );
    }
}
