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
        services.AddScoped<IBackgroundRepository, BackgroundRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ICharacterClassRepository, CharacterClassRepository>();
        services.AddScoped<IRogueRacketRepository, RogueRacketRepository>();
        services.AddScoped<IClericDoctrineRepository, ClericDoctrineRepository>();
        services.AddScoped<IDeityRepository, DeityRepository>();
        services.AddScoped<IClericDomainRepository, ClericDomainRepository>();
        services.AddScoped<IHuntersEdgeRepository, HuntersEdgeRepository>();
        services.AddScoped<IDruidicOrderRepository, DruidicOrderRepository>();
        services.AddScoped<IBardMuseRepository, BardMuseRepository>();
        services.AddScoped<IWitchPatronRepository, WitchPatronRepository>();
        services.AddScoped<IArcaneSchoolRepository, ArcaneSchoolRepository>();
        services.AddScoped<IArcaneThesisRepository, ArcaneThesisRepository>();
    }
}
