using Microsoft.Extensions.DependencyInjection;
using Secure.Application.Convertors;
using Secure.Application.Services.Authentication;

namespace Secure.Application;

public static class DependencyInjection
{
    public static void AddSecureApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( DependencyInjection ).Assembly ) );
        services.AddScoped<IRoleConvertor, RoleConvertor>();
        services.AddScoped<IUserConvertor, UserConvertor>();
    }
}