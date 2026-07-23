using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Shops;

public interface ISettlementRepository
{
    Task<Settlement?> GetAsync( int settlementId, CancellationToken cancellationToken );
    Task<Settlement?> GetByShopAsync( int shopId, CancellationToken cancellationToken );
    void Add( Settlement settlement );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
