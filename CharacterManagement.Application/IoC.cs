using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CharacterManagement.Application.Behaviors;
using Pathfinder.CharacterManagement.Application.Avatars;
using Pathfinder.CharacterManagement.Application.Builders;
using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.Completion;
using Pathfinder.CharacterManagement.Application.Features.Characters.Queries.Mapping;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application;

public static class DependencyInjection
{
    public static void AddCharacterManagementApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICharacterBuilder, CharacterBuilder>();
        services.AddSingleton<IAvatarCatalog, AvatarCatalog>();
        services.AddSingleton<IAvatarSelectionIndexProvider, RandomAvatarSelectionIndexProvider>();
        services.AddSingleton<IAvatarSelector, AvatarSelector>();
        services.AddSingleton<IAncestryChoiceAvailabilityPolicy, CommonAncestryChoiceAvailabilityPolicy>();
        services.AddScoped<CharacterDetailsDtoMapper>();
        services.AddScoped<CharacterCompletionEvaluator>();
        services.AddTransient<IValidator<CreateCharacterCommand>, CreateCharacterCommandValidator>();
        services.AddTransient<IValidator<SetCharacterGenderCommand>, SetCharacterGenderCommandValidator>();
        services.AddTransient<IValidator<ChangeHitPointsCommand>, ChangeHitPointsCommandValidator>();
        services.AddTransient( typeof( IPipelineBehavior<,> ), typeof( ValidationBehavior<,> ) );
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}
