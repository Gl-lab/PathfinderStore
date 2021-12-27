using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Shop;
using Pathfinder.Core.Repositories.Shop;

namespace Pathfinder.Application.Services;

public sealed class ShopService : IShopService
{
    private readonly IShopRepository _shopRepository;

    public ShopService(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;
    }

    public async Task<IReadOnlyList<Shop>> GetShopList() => await _shopRepository.ListAsync();

    public async Task<Shop> ShopById(int shopId)
    {
        return await _shopRepository.GetByIdAsync(shopId);
    }
}