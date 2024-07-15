using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Services;

public interface IWeaponService
{
    Task<ICollection<Weapon>> WeaponsByProductId(ICollection<int> productId);
}