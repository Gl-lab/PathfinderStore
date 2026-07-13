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
                BackgroundId = "background.acrobat",
                BackgroundRestrictedBoost = AbilityType.Dexterity,
                BackgroundFreeBoost = AbilityType.Charisma,
                ClassId = "class.fighter",
                ClassKeyAbility = AbilityType.Strength,
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
                BackgroundId = "background.acrobat",
                BackgroundRestrictedBoost = AbilityType.Dexterity,
                BackgroundFreeBoost = AbilityType.Charisma,
                ClassId = "class.fighter",
                ClassKeyAbility = AbilityType.Strength,
            } );

        validator.ValidateAndThrow( command );
    }

    [Fact]
    public void Validate_WhenBackgroundIdIsEmpty_ThrowsValidationException()
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
                BackgroundId = String.Empty,
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public void Validate_WhenBackgroundBoostIsMissing_ThrowsValidationException( bool omitRestrictedBoost )
    {
        CreateCharacterCommandValidator validator = new CreateCharacterCommandValidator();
        CreateCharacterRequestDto character = new CreateCharacterRequestDto
        {
            Name = "Thorin",
            AncestryType = AncestryType.Human,
            HeritageId = "human.skilled",
            AncestryFeatId = "human.cooperative_nature",
            FreeBoosts = [ AbilityType.Strength, AbilityType.Intelligence ],
            BackgroundId = "background.acrobat",
            BackgroundRestrictedBoost = omitRestrictedBoost ? null : AbilityType.Dexterity,
            BackgroundFreeBoost = omitRestrictedBoost ? AbilityType.Charisma : null,
        };
        CreateCharacterCommand command = new CreateCharacterCommand( 42, character );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public void Validate_WhenClassChoiceFieldIsMissing_ThrowsValidationException( bool omitClassId )
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
                BackgroundId = "background.acrobat",
                BackgroundRestrictedBoost = AbilityType.Dexterity,
                BackgroundFreeBoost = AbilityType.Charisma,
                ClassId = omitClassId ? String.Empty : "class.fighter",
                ClassKeyAbility = omitClassId ? AbilityType.Strength : null,
            } );

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow( command ) );
    }
}
