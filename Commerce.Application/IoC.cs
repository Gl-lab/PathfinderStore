using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Commerce.Domain.Restocking;

namespace Pathfinder.Commerce.Application;

public static class DependencyInjection
{
    public static void AddCommerceApplicationServices( this IServiceCollection services )
    {
        services.AddScoped<ShopAdministrationService>();
        services.AddScoped<ShopOfferAdministrationService>();
        services.AddScoped<WalletAdministrationService>();
        services.AddScoped<PurchaseReservationService>();
        services.AddScoped<RestockPolicyAdministrationService>();
        services.AddScoped<RestockGenerationService>();
        services.AddSingleton<DeterministicRestockSelector>();
        services.AddScoped<RestockRunLifecycleService>();
        services.AddSingleton( TimeProvider.System );
    }
}
