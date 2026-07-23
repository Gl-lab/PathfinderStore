using Pathfinder.Commerce.Domain.Offers;

namespace Pathfinder.Commerce.Application.Offers;

public sealed record ShopOfferDto(
    Guid OfferKey,
    int CampaignId,
    int ShopId,
    ShopOfferKind Kind,
    int? ItemConfigurationId,
    Guid? ItemInstanceKey,
    int AvailableQuantity,
    long UnitPriceCopper,
    ShopOfferStatus Status );
