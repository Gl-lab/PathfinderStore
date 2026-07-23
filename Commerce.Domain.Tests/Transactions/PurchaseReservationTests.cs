using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Transactions;

namespace Pathfinder.Commerce.Domain.Tests.Transactions;

public sealed class PurchaseReservationTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 23, 11, 0, 0, TimeSpan.Zero );

    [Fact]
    public void ReservesAndReleasesOfferAndWallet()
    {
        ShopOffer offer = ShopOffer.CreateCatalog( 7, 3, 19, 5, 100, _now );
        Wallet wallet = Wallet.Create( 7, 21, _now );
        wallet.ApplyAdjustment( Guid.NewGuid(), 1000, "Starting funds", 11, _now );
        Guid operationId = Guid.NewGuid();

        offer.Reserve( 2 );
        wallet.ReserveFunds( operationId, 200, 11, _now );

        Assert.Equal( 2, offer.ReservedQuantity );
        Assert.Equal( 200, wallet.ReservedCopper );
        Assert.Equal( 800, wallet.AvailableCopper );

        offer.Release( 2 );
        wallet.ReleaseFunds( Guid.NewGuid(), 200, 11, _now );

        Assert.Equal( 0, offer.ReservedQuantity );
        Assert.Equal( 0, wallet.ReservedCopper );
    }

    [Fact]
    public void CapturesOfferPrice()
    {
        PurchaseReservation reservation = PurchaseReservation.Create(
            Guid.NewGuid(),
            7,
            Guid.NewGuid(),
            21,
            3,
            125,
            _now,
            _now.AddMinutes( 15 ) );

        Assert.Equal( 125, reservation.UnitPriceCopper );
        Assert.Equal( 375, reservation.TotalPriceCopper );
    }
}
