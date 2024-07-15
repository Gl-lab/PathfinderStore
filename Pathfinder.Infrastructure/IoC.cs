using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Core.Repositories;
using Pathfinder.Core.Repositories.Shop;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Repository;
using Pathfinder.Infrastructure.Repository.Base;
using Pathfinder.Utils.Repositories.Base;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<ICharacterRepository, CharacterRepository>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRacesRepository, RacesRepository>();

            services.AddScoped<IWeaponItemPropertyRepository, WeaponItemPropertyRepository>();
            services.AddScoped<IWeaponRepository, WeaponRepository>();
            services.AddScoped<IShopRepository, ShopRepository>();
        }
    }
}