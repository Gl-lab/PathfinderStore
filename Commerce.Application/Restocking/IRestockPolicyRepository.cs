using Pathfinder.Commerce.Domain.Restocking;

namespace Pathfinder.Commerce.Application.Restocking;

public interface IRestockPolicyRepository
{
    Task<RestockPolicy?> GetByShopAsync( int shopId, CancellationToken cancellationToken );
    void Add( RestockPolicy policy );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
