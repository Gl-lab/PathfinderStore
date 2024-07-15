using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.Core.Repositories
{
    public interface IWeaponRepository: IRepository<Weapon>
    {
        Task<ICollection<Weapon>> GetDistinctCollectionByArticles(ICollection<int> articlesId);
    }
}