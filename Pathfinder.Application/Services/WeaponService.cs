using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Services;

public class WeaponService : IWeaponService
{
    private readonly IWeaponRepository _weaponRepository;

    public WeaponService(IWeaponRepository weaponRepository)
    {
        _weaponRepository = weaponRepository;
    }

    public async Task<ICollection<Weapon>> WeaponsByProductId(ICollection<int> productId)
    {
        return await _weaponRepository.GetDistinctCollectionByArticles(productId);
    }
}