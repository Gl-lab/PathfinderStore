using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Repository.Base;

namespace Pathfinder.Infrastructure.Repository
{
    public class WeaponItemPropertyRepository: Repository<WeaponItemProperty>, IWeaponItemPropertyRepository
    {
        public WeaponItemPropertyRepository(PgDbContext context) : base(context)
        {
        }

        public async Task<ICollection<WeaponItemProperty>> GetWeaponItemByItemIdCollection(ICollection<int> items)
        {
            return await TableNoTracking
                .Where(e => items.Contains(e.ItemId))
                .Include(e => e.Item)
                    .ThenInclude(item => item.Article)
                        .ThenInclude(article => article.Category)
                .Include(e => e.Item)
                    .ThenInclude(item => item.Article)
                        .ThenInclude(article => article.Effects)
                .Include(e => e.AdditionalDamages)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}