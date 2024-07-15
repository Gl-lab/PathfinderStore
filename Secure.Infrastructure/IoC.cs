using Authorization.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Secure.Infrastructure.Repositories;

namespace Secure.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddSecureInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPermissionsRepository, PermissionsRepository>();
            services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();
        }
    }
}