using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.Store.Application.Repositories;

public interface IWeaponItemPropertyRepository: IRepository<WeaponItemProperty>
{
    Task<ICollection<WeaponItemProperty>> GetWeaponItemByItemIdCollection(ICollection<int> items);
}