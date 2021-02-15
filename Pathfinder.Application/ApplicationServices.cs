using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Application.Services;
using Pathfinder.Application.Mapper;


namespace Pathfinder.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICharacterService, AccountService>();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddTransient<IRacesService, RacesService>();
        }
    }

}