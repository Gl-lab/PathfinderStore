using FluentValidation;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class ChangeHitPointsCommandValidatorTests
{
    [Theory]
    [InlineData( HitPointOperation.ApplyDamage )]
    [InlineData( HitPointOperation.Restore )]
    [InlineData( HitPointOperation.GrantTemporary )]
    public void Validate_PositiveAmountOperation_DoesNotThrow( HitPointOperation operation )
    {
        ChangeHitPointsCommandValidator validator = new ChangeHitPointsCommandValidator();

        validator.ValidateAndThrow( new ChangeHitPointsCommand( 1, 2, operation, 1 ) );
    }

    [Fact]
    public void Validate_ClearTemporaryWithZeroAmount_DoesNotThrow()
    {
        ChangeHitPointsCommandValidator validator = new ChangeHitPointsCommandValidator();

        validator.ValidateAndThrow( new ChangeHitPointsCommand(
            1,
            2,
            HitPointOperation.ClearTemporary,
            0 ) );
    }

    [Theory]
    [InlineData( HitPointOperation.ApplyDamage, 0 )]
    [InlineData( HitPointOperation.Restore, -1 )]
    [InlineData( HitPointOperation.GrantTemporary, 0 )]
    [InlineData( HitPointOperation.ClearTemporary, 1 )]
    [InlineData( ( HitPointOperation )99, 1 )]
    public void Validate_InvalidOperationOrAmount_ThrowsValidationException(
        HitPointOperation operation,
        int amount )
    {
        ChangeHitPointsCommandValidator validator = new ChangeHitPointsCommandValidator();

        Assert.Throws<ValidationException>( () => validator.ValidateAndThrow(
            new ChangeHitPointsCommand( 1, 2, operation, amount ) ) );
    }
}
