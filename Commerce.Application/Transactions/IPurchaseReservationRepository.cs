using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Transactions;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Transactions;

public interface IPurchaseReservationRepository
{
    Task<ShopOffer?> GetOfferAsync(
        int campaignId,
        Guid offerKey,
        CancellationToken cancellationToken );
    Task<Shop?> GetShopAsync(
        int campaignId,
        int shopId,
        CancellationToken cancellationToken );
    Task<Wallet?> GetWalletAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken );
    Task<PurchaseReservation?> GetByOperationAsync(
        int campaignId,
        Guid operationId,
        CancellationToken cancellationToken );
    Task<PurchaseReservation?> GetAsync(
        int campaignId,
        Guid reservationKey,
        CancellationToken cancellationToken );
    Task<ShopSale?> GetSaleByOperationAsync(
        int campaignId,
        Guid operationId,
        CancellationToken cancellationToken );
    void Add( PurchaseReservation reservation );
    void AddSale( ShopSale sale );
    void AddWallet( Wallet wallet );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
