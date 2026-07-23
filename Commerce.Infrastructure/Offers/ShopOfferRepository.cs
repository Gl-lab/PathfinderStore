using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Commerce.Infrastructure.Offers;

public sealed class ShopOfferRepository : IShopOfferRepository
{
    private readonly CommerceDbContext _dbContext;

    public ShopOfferRepository( CommerceDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<Shop?> GetShopAsync(
        int shopId,
        CancellationToken cancellationToken ) => _dbContext.Shops
        .SingleOrDefaultAsync( shop => shop.Id == shopId, cancellationToken );

    public Task<bool> HasActiveInstanceOfferAsync(
        Guid itemInstanceKey,
        CancellationToken cancellationToken ) => _dbContext.ShopOffers.AnyAsync(
        offer =>
            ( offer.ItemInstanceKey == itemInstanceKey ) &&
            ( offer.Status == ShopOfferStatus.Active ),
        cancellationToken );

    public void Add( ShopOffer offer )
    {
        _dbContext.ShopOffers.Add( offer );
    }

    public Task SaveChangesAsync( CancellationToken cancellationToken ) =>
        _dbContext.SaveChangesAsync( cancellationToken );
}
