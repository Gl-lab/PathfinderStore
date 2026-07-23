using Pathfinder.Commerce.Domain.Transactions;

namespace Pathfinder.Commerce.Application.Transactions;

public sealed record PurchaseReservationDto(
    Guid ReservationKey,
    Guid OperationId,
    int CampaignId,
    Guid OfferKey,
    int BuyerCharacterId,
    int Quantity,
    long UnitPriceCopper,
    long TotalPriceCopper,
    PurchaseReservationStatus Status,
    DateTimeOffset ExpiresAtUtc );
