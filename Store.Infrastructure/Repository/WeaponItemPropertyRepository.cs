using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Store.Infrastructure.Data;

namespace Pathfinder.Store.Infrastructure.Repository;

public class WeaponItemPropertyRepository : Repository<WeaponItemProperty>, IWeaponItemPropertyRepository
{
    public WeaponItemPropertyRepository(StoreDbContext context) : base(context)
    {
    }

    public async Task<ICollection<WeaponItemProperty>> GetWeaponItemByItemIdCollection(ICollection<int> items)
    {
        return await TableNoTracking
                    .Where(e => items.Contains(e.ItemId))
                    .Include(e => e.Item)
                    .ThenInclude(item => item.Product)
                     //   .ThenInclude(article => article.Category)
                    .Include(e => e.Item)
                    .ThenInclude(item => item.Product)
                     //   .ThenInclude(article => article.Effects)
                    .Include(e => e.AdditionalDamages)
                    .ToListAsync()
                    .ConfigureAwait(false);
    }
}