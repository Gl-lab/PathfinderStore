using System;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Transactions;

namespace Pathfinder.Web.Integration;

public enum CommerceOfferStatusFilter
{
    Active = 1,
    All = 2,
}

public sealed record CommerceShopOfferDto(
    Guid OfferKey,
    int CampaignId,
    int ShopId,
    ShopOfferKind Kind,
    int ItemConfigurationId,
    Guid? ItemInstanceKey,
    string ItemName,
    int ItemLevel,
    int AvailableQuantity,
    int ReservedQuantity,
    long UnitPriceCopper,
    ShopOfferStatus Status,
    int Version );

public sealed record CommercePurchaseReservationDto(
    Guid ReservationKey,
    Guid OperationId,
    int CampaignId,
    Guid OfferKey,
    int BuyerCharacterId,
    string ItemName,
    int Quantity,
    long UnitPriceCopper,
    long TotalPriceCopper,
    PurchaseReservationStatus Status,
    DateTimeOffset ExpiresAtUtc,
    Guid? PurchasedItemInstanceKey );

public sealed record CommerceSellQuoteDto(
    Guid ItemInstanceKey,
    int ItemConfigurationId,
    string ItemName,
    int Quantity,
    long UnitPriceCopper,
    long TotalPriceCopper );
