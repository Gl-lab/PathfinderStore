using Microsoft.Extensions.DependencyInjection;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Configurations;

namespace Pathfinder.ItemCatalog.Application;

public static class DependencyInjection
{
    public static void AddItemCatalogApplicationServices( this IServiceCollection services )
    {
        services.AddScoped<ItemCatalogAdministrationService>();
        services.AddScoped<ItemConfigurationAdministrationService>();
        services.AddSingleton( TimeProvider.System );
    }
}