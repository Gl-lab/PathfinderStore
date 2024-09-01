using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CharacterManagement.Application.Services;
using Pathfinder.CharacterManagement.Application.Services.Implementation;

namespace Pathfinder.CharacterManagement.Application;

public static class DependencyInjection
{
    public static void AddCharacterManagementApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IRacesService, RacesService>();
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}