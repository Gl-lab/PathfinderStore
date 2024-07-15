using CharacterManagement.Application.Services;
using CharacterManagement.Application.Services.Implementation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CharacterManagement.Application;

public static class DependencyInjection
{
    public static void AddCharacterManagementApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IRacesService, RacesService>();
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}