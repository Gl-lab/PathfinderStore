using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Commerce.Application.Shops;

namespace Pathfinder.Commerce.Application;

public static class DependencyInjection
{
    public static void AddCommerceApplicationServices( this IServiceCollection services )
    {
        services.AddScoped<ShopAdministrationService>();
        services.AddSingleton( TimeProvider.System );
    }
}
