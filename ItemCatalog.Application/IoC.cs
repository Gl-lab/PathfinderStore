using Microsoft.Extensions.DependencyInjection;
using Pathfinder.ItemCatalog.Application.Administration;

namespace Pathfinder.ItemCatalog.Application;

public static class DependencyInjection
{
    public static void AddItemCatalogApplicationServices( this IServiceCollection services )
    {
        services.AddScoped<ItemCatalogAdministrationService>();
        services.AddSingleton( TimeProvider.System );
    }
}