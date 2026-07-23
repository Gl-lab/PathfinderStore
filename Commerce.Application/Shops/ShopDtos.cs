namespace Pathfinder.Commerce.Application.Shops;

public sealed record ShopDto(
    int Id,
    int CampaignId,
    int SettlementId,
    string Name,
    string Specialization,
    int ShopLevel,
    int CatalogPricePercent,
    int BuybackPricePercent,
    int PricingPolicyVersion );

public sealed record UpdateShopPricingPolicyRequest(
    int CampaignId,
    int ShopId,
    int CatalogPricePercent,
    int BuybackPricePercent,
    int ActingUserId );

public sealed record SettlementDto(
    int Id,
    int CampaignId,
    string Name,
    int Level,
    string Region,
    string Traits,
    IReadOnlyCollection<ShopDto> Shops );

public sealed record CreateSettlementRequest(
    int CampaignId,
    string Name,
    int Level,
    string Region,
    string Traits,
    int ActingUserId );

public sealed record CreateShopRequest(
    int CampaignId,
    int SettlementId,
    string Name,
    string Specialization,
    int ShopLevel,
    int ActingUserId );
