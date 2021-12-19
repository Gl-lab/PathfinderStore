using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Application.Services;
using Pathfinder.Application.Mapper;
using Pathfinder.Application.Services.Authentication;


namespace Pathfinder.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICharacterService, CharacterService>();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddScoped<IRacesService, RacesService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddMediatR(typeof(DependencyInjection), typeof(AutoMapperProfile));
        }
    }

}