using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Transactions;

namespace Pathfinder.Commerce.Application.Transactions;

public sealed class PurchaseReservationService
{
    private static readonly TimeSpan ReservationLifetime = TimeSpan.FromMinutes( 15 );

    private readonly IPurchaseReservationRepository _repository;
    private readonly ICommerceBuyerAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public PurchaseReservationService(
        IPurchaseReservationRepository repository,
        ICommerceBuyerAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<PurchaseReservationDto> ReserveAsync(
        int campaignId,
        Guid operationId,
        Guid offerKey,
        int buyerCharacterId,
        int quantity,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureControlsCharacterAsync(
            campaignId,
            actingUserId,
            buyerCharacterId,
            cancellationToken );
        PurchaseReservation? existing = await _repository.GetByOperationAsync(
            campaignId,
            operationId,
            cancellationToken );
        if ( existing is not null )
        {
            EnsureSameRequest( existing, offerKey, buyerCharacterId, quantity );
            return ToDto( existing );
        }

        ShopOffer offer = await _repository.GetOfferAsync(
            campaignId,
            offerKey,
            cancellationToken ) ?? throw new CommerceException( "Active offer was not found." );
        Wallet wallet = await _repository.GetWalletAsync(
            campaignId,
            buyerCharacterId,
            cancellationToken ) ?? throw new CommerceException( "Buyer wallet was not found." );
        DateTimeOffset now = _timeProvider.GetUtcNow();
        PurchaseReservation reservation = PurchaseReservation.Create(
            operationId,
            campaignId,
            offerKey,
            buyerCharacterId,
            quantity,
            offer.UnitPriceCopper,
            now,
            now.Add( ReservationLifetime ) );
        offer.Reserve( quantity );
        if ( reservation.TotalPriceCopper > 0 )
        {
            wallet.ReserveFunds(
                operationId,
                reservation.TotalPriceCopper,
                actingUserId,
                now );
        }

        _repository.Add( reservation );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( reservation );
    }

    public async Task<PurchaseReservationDto> CancelAsync(
        int campaignId,
        Guid reservationKey,
        Guid releaseOperationId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        PurchaseReservation reservation = await _repository.GetAsync(
            campaignId,
            reservationKey,
            cancellationToken ) ?? throw new CommerceException( "Reservation was not found." );
        await EnsureControlsCharacterAsync(
            campaignId,
            actingUserId,
            reservation.BuyerCharacterId,
            cancellationToken );
        if ( reservation.Status == PurchaseReservationStatus.Cancelled )
        {
            return ToDto( reservation );
        }

        ShopOffer offer = await _repository.GetOfferAsync(
            campaignId,
            reservation.OfferKey,
            cancellationToken ) ?? throw new CommerceException( "Reserved offer was not found." );
        Wallet wallet = await _repository.GetWalletAsync(
            campaignId,
            reservation.BuyerCharacterId,
            cancellationToken ) ?? throw new CommerceException( "Buyer wallet was not found." );
        offer.Release( reservation.Quantity );
        if ( reservation.TotalPriceCopper > 0 )
        {
            wallet.ReleaseFunds(
                releaseOperationId,
                reservation.TotalPriceCopper,
                actingUserId,
                _timeProvider.GetUtcNow() );
        }

        reservation.Cancel( _timeProvider.GetUtcNow() );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( reservation );
    }

    private async Task EnsureControlsCharacterAsync(
        int campaignId,
        int actingUserId,
        int characterId,
        CancellationToken cancellationToken )
    {
        bool controlsCharacter = await _accessPolicy.ControlsCharacterAsync(
            campaignId,
            actingUserId,
            characterId,
            cancellationToken );
        if ( !controlsCharacter )
        {
            throw new UnauthorizedAccessException(
                "Only the active player controlling this character can reserve a purchase." );
        }
    }

    private static void EnsureSameRequest(
        PurchaseReservation existing,
        Guid offerKey,
        int buyerCharacterId,
        int quantity )
    {
        if ( ( existing.OfferKey != offerKey ) ||
             ( existing.BuyerCharacterId != buyerCharacterId ) ||
             ( existing.Quantity != quantity ) )
        {
            throw new CommerceException(
                "Purchase operation id was already used with different data." );
        }
    }

    private static PurchaseReservationDto ToDto( PurchaseReservation reservation ) =>
        new PurchaseReservationDto(
            reservation.ReservationKey,
            reservation.OperationId,
            reservation.CampaignId,
            reservation.OfferKey,
            reservation.BuyerCharacterId,
            reservation.Quantity,
            reservation.UnitPriceCopper,
            reservation.TotalPriceCopper,
            reservation.Status,
            reservation.ExpiresAtUtc );
}
