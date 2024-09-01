using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Secure.Application.Repositories;
using Pathfinder.Secure.Infrastructure.Repositories;

namespace Pathfinder.Secure.Infrastructure;

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