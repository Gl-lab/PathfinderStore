using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Shops;

internal static class ShopMappings
{
    public static SettlementDto ToDto( this Settlement settlement ) => new SettlementDto(
        settlement.Id,
        settlement.CampaignId,
        settlement.Name,
        settlement.Level,
        settlement.Region,
        settlement.Traits,
        settlement.Shops
            .Select( shop => shop.ToDto() )
            .ToArray() );

    public static ShopDto ToDto( this Shop shop ) => new ShopDto(
        shop.Id,
        shop.CampaignId,
        shop.SettlementId,
        shop.Name,
        shop.Specialization,
        shop.ShopLevel );
}
