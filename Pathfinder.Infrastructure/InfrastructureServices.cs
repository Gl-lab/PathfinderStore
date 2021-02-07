using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Core.Repositories;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Core.Repositories.Base;
using Pathfinder.Infrastructure.Repository;
using Pathfinder.Infrastructure.Repository.Auth;
using Pathfinder.Infrastructure.Repository.Base;


namespace Pathfinder.Application
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
          
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPermissionsRepository, PermissionsRepository>();
            services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();
        }
    }

}