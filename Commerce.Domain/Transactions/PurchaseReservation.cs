using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Transactions;

public sealed class PurchaseReservation : Entity, IAggregateRoot
{
    private PurchaseReservation()
    {
    }

    public Guid ReservationKey { get; private set; }
    public Guid OperationId { get; private set; }
    public int CampaignId { get; private set; }
    public Guid OfferKey { get; private set; }
    public int BuyerCharacterId { get; private set; }
    public int Quantity { get; private set; }
    public long UnitPriceCopper { get; private set; }
    public long TotalPriceCopper { get; private set; }
    public PurchaseReservationStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? ClosedAtUtc { get; private set; }

    public static PurchaseReservation Create(
        Guid operationId,
        int campaignId,
        Guid offerKey,
        int buyerCharacterId,
        int quantity,
        long unitPriceCopper,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc )
    {
        if ( ( operationId == Guid.Empty ) || ( offerKey == Guid.Empty ) )
        {
            throw new CommerceException( "Reservation operation and offer keys cannot be empty." );
        }

        if ( ( campaignId <= 0 ) || ( buyerCharacterId <= 0 ) || ( quantity <= 0 ) )
        {
            throw new CommerceException( "Reservation ids and quantity must be greater than zero." );
        }

        if ( ( unitPriceCopper < 0 ) || ( expiresAtUtc <= createdAtUtc ) )
        {
            throw new CommerceException( "Reservation price or expiration is invalid." );
        }

        return new PurchaseReservation
        {
            ReservationKey = Guid.NewGuid(),
            OperationId = operationId,
            CampaignId = campaignId,
            OfferKey = offerKey,
            BuyerCharacterId = buyerCharacterId,
            Quantity = quantity,
            UnitPriceCopper = unitPriceCopper,
            TotalPriceCopper = checked( unitPriceCopper * quantity ),
            Status = PurchaseReservationStatus.Active,
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc,
        };
    }

    public void Cancel( DateTimeOffset cancelledAtUtc )
    {
        if ( Status == PurchaseReservationStatus.Cancelled )
        {
            return;
        }

        if ( Status != PurchaseReservationStatus.Active )
        {
            throw new CommerceException( "Only an active reservation can be cancelled." );
        }

        Status = PurchaseReservationStatus.Cancelled;
        ClosedAtUtc = cancelledAtUtc;
    }
}
