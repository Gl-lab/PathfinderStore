using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Infrastructure.Shops;

namespace Pathfinder.Commerce.Infrastructure;

public static class DependencyInjection
{
    public static void AddCommerceInfrastructureServices( this IServiceCollection services )
    {
        services.AddScoped<ISettlementRepository, SettlementRepository>();
    }
}
