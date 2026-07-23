using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Transactions;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Commerce.Infrastructure.Transactions;

public sealed class PurchaseReservationRepository : IPurchaseReservationRepository
{
    private readonly CommerceDbContext _dbContext;

    public PurchaseReservationRepository( CommerceDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<ShopOffer?> GetOfferAsync(
        int campaignId,
        Guid offerKey,
        CancellationToken cancellationToken ) => _dbContext.ShopOffers.SingleOrDefaultAsync(
        offer =>
            ( offer.CampaignId == campaignId ) &&
            ( offer.OfferKey == offerKey ) &&
            ( offer.Status == ShopOfferStatus.Active ),
        cancellationToken );

    public Task<Wallet?> GetWalletAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken ) => _dbContext.Wallets
        .Include( wallet => wallet.Entries )
        .SingleOrDefaultAsync(
            wallet =>
                ( wallet.CampaignId == campaignId ) &&
                ( wallet.CharacterId == characterId ),
            cancellationToken );

    public Task<PurchaseReservation?> GetByOperationAsync(
        int campaignId,
        Guid operationId,
        CancellationToken cancellationToken ) => _dbContext.PurchaseReservations
        .SingleOrDefaultAsync(
            reservation =>
                ( reservation.CampaignId == campaignId ) &&
                ( reservation.OperationId == operationId ),
            cancellationToken );

    public Task<PurchaseReservation?> GetAsync(
        int campaignId,
        Guid reservationKey,
        CancellationToken cancellationToken ) => _dbContext.PurchaseReservations
        .SingleOrDefaultAsync(
            reservation =>
                ( reservation.CampaignId == campaignId ) &&
                ( reservation.ReservationKey == reservationKey ),
            cancellationToken );

    public void Add( PurchaseReservation reservation )
    {
        _dbContext.PurchaseReservations.Add( reservation );
    }

    public Task SaveChangesAsync( CancellationToken cancellationToken ) =>
        _dbContext.SaveChangesAsync( cancellationToken );
}
