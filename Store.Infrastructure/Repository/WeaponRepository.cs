using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Store.Infrastructure.Data;

namespace Pathfinder.Store.Infrastructure.Repository;

public class WeaponRepository : Repository<Weapon>, IWeaponRepository
{
    public WeaponRepository(StoreDbContext context) : base(context)
    {
    }

    public async Task<ICollection<Weapon>> GetDistinctCollectionByArticles(ICollection<int> articlesId)
    {
        return await TableNoTracking
                    .Where(weapon => articlesId.Contains(weapon
                                                        .Product.Id))
                    .Include(e => e.Product)
                    .Include(e => e.Ammunition)
                    .Include(e => e.DamageTypeList)
                    .Include(e => e.MediumSizeDamage)
                    .Include(e => e.WeaponType)
                    .ThenInclude(weaponType => weaponType.WeaponProficiency)
                    .Include(e => e.SmallSizeDamage)
                    .ToListAsync();
    }
}