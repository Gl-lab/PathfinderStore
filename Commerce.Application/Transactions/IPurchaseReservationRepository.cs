using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Transactions;

namespace Pathfinder.Commerce.Application.Transactions;

public interface IPurchaseReservationRepository
{
    Task<ShopOffer?> GetOfferAsync(
        int campaignId,
        Guid offerKey,
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
    void Add( PurchaseReservation reservation );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
