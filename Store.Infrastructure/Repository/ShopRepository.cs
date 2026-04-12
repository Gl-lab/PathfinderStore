using Pathfinder.Contracts.Core.Entities.Shop;
using Pathfinder.Store.Application.Repositories.Shop;
using Pathfinder.Store.Infrastructure.Data;

namespace Pathfinder.Store.Infrastructure.Repository;

internal sealed class ShopRepository : Repository<Shop>, IShopRepository
{
    public ShopRepository( StoreDbContext context ) : base( context )
    {
    }
}