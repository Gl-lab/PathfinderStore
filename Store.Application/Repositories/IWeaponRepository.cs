using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.Store.Application.Repositories;

public interface IWeaponRepository: IRepository<Weapon>
{
    Task<ICollection<Weapon>> GetDistinctCollectionByArticles(ICollection<int> articlesId);
}