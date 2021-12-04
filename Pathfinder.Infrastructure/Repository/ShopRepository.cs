using Pathfinder.Core.Entities.Shop;
using Pathfinder.Core.Repositories.Shop;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Repository.Base;

namespace Pathfinder.Infrastructure.Repository;

internal sealed class ShopRepository : Repository<Shop>, IShopRepository
{
    public ShopRepository(PgDbContext context) : base(context)
    {
    }
}
