using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Domain.Tests.Shops;

public sealed class SettlementTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 23, 8, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateScopesSettlementAndShopToCampaign()
    {
        Settlement settlement = Settlement.Create(
            17,
            " Otari ",
            4,
            "Isle of Kortos",
            "Coastal",
            _now );

        Shop shop = settlement.AddShop( "Wrin's Wonders", "Occult", 5, _now );

        Assert.Equal( 17, settlement.CampaignId );
        Assert.Equal( "Otari", settlement.Name );
        Assert.Equal( 17, shop.CampaignId );
        Assert.Single( settlement.Shops );
    }

    [Fact]
    public void AddShopRejectsDuplicateName()
    {
        Settlement settlement = Settlement.Create(
            17,
            "Otari",
            4,
            String.Empty,
            String.Empty,
            _now );
        settlement.AddShop( "Market", String.Empty, 4, _now );

        CommerceException exception = Assert.Throws<CommerceException>(
            () => settlement.AddShop( " market ", String.Empty, 4, _now ) );

        Assert.Contains( "already contains", exception.Message );
    }

    [Fact]
    public void PricingPolicyCalculatesRetailAndBuybackWithoutChangingPastValues()
    {
        Settlement settlement = Settlement.Create(
            17,
            "Otari",
            4,
            String.Empty,
            String.Empty,
            _now );
        Shop shop = settlement.AddShop( "Market", String.Empty, 4, _now );
        long originalRetail = shop.CalculateCatalogPrice( 1000 );

        shop.SetPricingPolicy( 125, 40 );

        Assert.Equal( 1000, originalRetail );
        Assert.Equal( 1250, shop.CalculateCatalogPrice( 1000 ) );
        Assert.Equal( 400, shop.CalculateBuybackPrice( 1000 ) );
        Assert.Equal( 2, shop.PricingPolicyVersion );
    }
}
