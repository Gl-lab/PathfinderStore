using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace Pathfinder.CharacterManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddCharacterManagementInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<IAncestryRepository, AncestryRepository>();
    }
}
