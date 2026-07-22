using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Infrastructure.Transfers;

namespace Pathfinder.Inventory.Infrastructure;

public static class DependencyInjection
{
    public static void AddInventoryInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<IInventoryTransferRepository, InventoryTransferRepository>();
    }
}
