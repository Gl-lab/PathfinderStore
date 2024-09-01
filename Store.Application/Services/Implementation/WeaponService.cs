using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Store.Application.Repositories;

namespace Pathfinder.Store.Application.Services.Implementation;

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