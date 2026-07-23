using Pathfinder.Commerce.Domain.Offers;

namespace Pathfinder.Commerce.Domain.Tests.Offers;

public sealed class ShopOfferTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 23, 9, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CatalogOfferReferencesOnlyConfiguration()
    {
        ShopOffer offer = ShopOffer.CreateCatalog( 1, 2, 3, 4, 125, _now );

        Assert.Equal( ShopOfferKind.Catalog, offer.Kind );
        Assert.Equal( 3, offer.ItemConfigurationId );
        Assert.Null( offer.ItemInstanceKey );
        Assert.Equal( 4, offer.AvailableQuantity );
    }

    [Fact]
    public void StockOfferReferencesOnlyInstance()
    {
        Guid instanceKey = Guid.NewGuid();

        ShopOffer offer = ShopOffer.CreateStockInstance( 1, 2, instanceKey, 1, 500, _now );

        Assert.Equal( ShopOfferKind.StockInstance, offer.Kind );
        Assert.Null( offer.ItemConfigurationId );
        Assert.Equal( instanceKey, offer.ItemInstanceKey );
    }
}
