using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories.Base;

namespace Pathfinder.Core.Repositories
{
    public interface IWeaponItemPropertyRepository: IRepository<WeaponItemProperty>
    {
        Task<ICollection<WeaponItemProperty>> GetWeaponItemByItemIdCollection(ICollection<int> items);
    }
}