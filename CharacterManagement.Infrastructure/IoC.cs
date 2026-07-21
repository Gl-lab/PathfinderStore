using Microsoft.Extensions.DependencyInjection;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Infrastructure.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace Pathfinder.CharacterManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddCharacterManagementInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<IAncestryRepository, AncestryRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<IBackgroundRepository, BackgroundRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ICharacterClassRepository, CharacterClassRepository>();
        services.AddScoped<IRogueRacketRepository, RogueRacketRepository>();
        services.AddScoped<IClericDoctrineRepository, ClericDoctrineRepository>();
        services.AddScoped<IDeityRepository, DeityRepository>();
        services.AddScoped<IClericDomainRepository, ClericDomainRepository>();
        services.AddScoped<ISpellRepository, SpellRepository>();
        services.AddScoped<IFeatRepository, FeatRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IAllowedEquipmentReader, StartingEquipmentAllowedEquipmentReader>();
        services.AddScoped<IHuntersEdgeRepository, HuntersEdgeRepository>();
        services.AddScoped<IDruidicOrderRepository, DruidicOrderRepository>();
        services.AddScoped<IBardMuseRepository, BardMuseRepository>();
        services.AddScoped<IWitchPatronRepository, WitchPatronRepository>();
        services.AddScoped<IArcaneSchoolRepository, ArcaneSchoolRepository>();
        services.AddScoped<IArcaneThesisRepository, ArcaneThesisRepository>();
    }
}
