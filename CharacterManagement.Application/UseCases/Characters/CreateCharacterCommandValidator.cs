using FluentValidation;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor( command => command.UserId )
            .GreaterThan( 0 );

        RuleFor( command => command.Character )
            .NotNull();

        When(
            command => command.Character is not null,
            () =>
            {
                RuleFor( command => command.Character.Name )
                    .NotEmpty();

                RuleFor( command => command.Character.AncestryType )
                    .NotEqual( AncestryType.None );

                RuleFor( command => command.Character.HeritageId )
                    .NotEmpty();

                RuleFor( command => command.Character.AncestryFeatId )
                    .NotEmpty();

                RuleFor( command => command.Character.FreeBoosts )
                    .NotNull();

                RuleFor( command => command.Character.BackgroundId )
                    .NotEmpty();

                RuleFor( command => command.Character.BackgroundRestrictedBoost )
                    .NotNull();

                RuleFor( command => command.Character.BackgroundFreeBoost )
                    .NotNull();
            } );
    }
}
