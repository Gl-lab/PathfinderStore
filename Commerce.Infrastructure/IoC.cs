using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Infrastructure.Shops;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Infrastructure.Offers;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Infrastructure.Money;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Infrastructure.Transactions;

namespace Pathfinder.Commerce.Infrastructure;

public static class DependencyInjection
{
    public static void AddCommerceInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<ISettlementRepository, SettlementRepository>();
        services.AddScoped<IShopOfferRepository, ShopOfferRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IPurchaseReservationRepository, PurchaseReservationRepository>();
    }
}
