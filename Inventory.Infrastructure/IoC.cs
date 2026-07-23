using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Infrastructure.Transfers;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Inventory.Infrastructure.Commerce;

namespace Pathfinder.Inventory.Infrastructure;

public static class DependencyInjection
{
    public static void AddInventoryInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<IInventoryTransferRepository, InventoryTransferRepository>();
        services.AddScoped<ICommerceInventoryReader, CommerceInventoryReader>();
    }
}
