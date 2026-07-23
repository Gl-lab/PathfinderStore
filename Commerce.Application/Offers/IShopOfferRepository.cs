using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Offers;

public interface IShopOfferRepository
{
    Task<Shop?> GetShopAsync( int shopId, CancellationToken cancellationToken );
    Task<bool> HasActiveInstanceOfferAsync(
        Guid itemInstanceKey,
        CancellationToken cancellationToken );
    void Add( ShopOffer offer );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
