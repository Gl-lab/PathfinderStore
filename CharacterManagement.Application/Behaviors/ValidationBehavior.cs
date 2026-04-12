using FluentValidation;
using MediatR;

namespace Pathfinder.CharacterManagement.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior( IEnumerable<IValidator<TRequest>> validators )
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken )
    {
        if ( _validators.Any() )
        {
            ValidationContext<TRequest> context = new ValidationContext<TRequest>( request );
            List<FluentValidation.Results.ValidationFailure> failures = _validators
                .Select( validator => validator.Validate( context ) )
                .SelectMany( validationResult => validationResult.Errors )
                .Where( validationFailure => validationFailure is not null )
                .ToList();

            if ( failures.Count > 0 )
            {
                throw new ValidationException( failures );
            }
        }

        return await next();
    }
}
