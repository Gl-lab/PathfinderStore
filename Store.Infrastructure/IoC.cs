using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Store.Infrastructure.Repository;

namespace Pathfinder.Store.Infrastructure;

public static class DependencyInjection
{
    public static void AddStoreInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IWeaponItemPropertyRepository, WeaponItemPropertyRepository>();
        services.AddScoped<IWeaponRepository, WeaponRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
    }
}