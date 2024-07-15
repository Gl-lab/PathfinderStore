using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Shop;

namespace Pathfinder.Application.Services
{
    public interface IShopService
    {
        Task<IReadOnlyList<Shop>> GetShopList();
        Task<Shop> ShopById(int shopId);
    }
}