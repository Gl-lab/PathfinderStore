using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Application.Services;
using Pathfinder.Application.Services.Implementation;

namespace Pathfinder.Application;

public static class DependencyInjection
{
    public static void AddShopApplicationServices( this IServiceCollection services )
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IWeaponService, WeaponService>();
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
    }
}