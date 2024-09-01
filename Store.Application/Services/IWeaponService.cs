using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Store.Application.Services;

public interface IWeaponService
{
    Task<ICollection<Weapon>> WeaponsByProductId(ICollection<int> productId);
}