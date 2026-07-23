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
    DateTimeOffset ExpiresAtUtc,
    Guid? PurchasedItemInstanceKey );

public sealed record ShopSaleDto(
    Guid SaleKey,
    Guid OperationId,
    int CampaignId,
    int ShopId,
    int SellerCharacterId,
    Guid ItemInstanceKey,
    int Quantity,
    long UnitPriceCopper,
    long TotalPriceCopper,
    DateTimeOffset CompletedAtUtc );
