using FluentValidation;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class SetCharacterGenderCommandValidator : AbstractValidator<SetCharacterGenderCommand>
{
    public SetCharacterGenderCommandValidator()
    {
        RuleFor( command => command.UserId )
            .GreaterThan( 0 );

        RuleFor( command => command.CharacterId )
            .GreaterThan( 0 );

        RuleFor( command => command.Gender )
            .Must( gender =>
                gender == CharacterGender.Male ||
                gender == CharacterGender.Female )
            .WithMessage( "Character gender must be Male or Female." );
    }
}
