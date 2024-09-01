using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Contracts.Core.Entities.Shop;

namespace Pathfinder.Store.Application.Services;

public interface IShopService
{
    Task<IReadOnlyList<Shop>> GetShopList();
    Task<Shop> ShopById(int shopId);
}