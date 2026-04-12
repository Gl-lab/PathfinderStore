using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Store.Application.Services;
using Pathfinder.Store.Application.Services.Implementation;

namespace Pathfinder.Store.Application;

public static class DependencyInjection
{
    public static void AddStoreApplication( this IServiceCollection services )
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
       
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IWeaponService, WeaponService>();
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}