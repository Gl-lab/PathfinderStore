using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Application.Offers;

namespace Pathfinder.Commerce.Application;

public static class DependencyInjection
{
    public static void AddCommerceApplicationServices( this IServiceCollection services )
    {
        services.AddScoped<ShopAdministrationService>();
        services.AddScoped<ShopOfferAdministrationService>();
        services.AddSingleton( TimeProvider.System );
    }
}
