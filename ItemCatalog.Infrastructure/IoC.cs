using Microsoft.Extensions.DependencyInjection;
using Pathfinder.ItemCatalog.Application.Items;
using Pathfinder.ItemCatalog.Infrastructure.Items;

namespace Pathfinder.ItemCatalog.Infrastructure;

public static class DependencyInjection
{
    public static void AddItemCatalogInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();
    }
}