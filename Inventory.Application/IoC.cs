using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Pathfinder.Inventory.Application;

public static class DependencyInjection
{
    public static void AddInventoryApplicationServices( this IServiceCollection services )
    {
        services.AddSingleton( TimeProvider.System );
        services.AddMediatR( configuration =>
            configuration.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}
