using FluentValidation;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class ChangeHitPointsCommandValidator : AbstractValidator<ChangeHitPointsCommand>
{
    public ChangeHitPointsCommandValidator()
    {
        RuleFor( command => command.UserId ).GreaterThan( 0 );
        RuleFor( command => command.CharacterId ).GreaterThan( 0 );
        RuleFor( command => command.Operation ).IsInEnum();
        When(
            command => command.Operation == HitPointOperation.ClearTemporary,
            () => RuleFor( command => command.Amount ).Equal( 0 ) );
        When(
            command => command.Operation != HitPointOperation.ClearTemporary,
            () => RuleFor( command => command.Amount ).GreaterThan( 0 ) );
    }
}
