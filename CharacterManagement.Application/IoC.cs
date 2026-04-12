using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CharacterManagement.Application.Behaviors;
using Pathfinder.CharacterManagement.Application.Builders;
using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.Converters.Implementation;
using Pathfinder.CharacterManagement.Application.Services;
using Pathfinder.CharacterManagement.Application.Services.Implementation;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;

namespace Pathfinder.CharacterManagement.Application;

public static class DependencyInjection
{
    public static void AddCharacterManagementApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<ICharacterBuilder, CharacterBuilder>();
        services.AddScoped<ICharacterConvertor, CharacterConvertor>();
        services.AddScoped<IAccountConvertor, AccountConvertor>();
        services.AddTransient<IValidator<CreateCharacterCommand>, CreateCharacterCommandValidator>();
        services.AddTransient( typeof( IPipelineBehavior<,> ), typeof( ValidationBehavior<,> ) );
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}
