using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Transactions;
using Pathfinder.Commerce.Application.Offers;

namespace Pathfinder.Commerce.Infrastructure.Tests.Transactions;

public sealed class PurchaseReservationServiceTests
{
    [Fact]
    public async Task ReserveAndCancelAreAtomicWithinCommerceContext()
    {
        TestState state = await TestState.CreateAsync();
        PurchaseReservationService service = new PurchaseReservationService(
            new PurchaseReservationRepository( state.DbContext ),
            new StubBuyerAccessPolicy(),
            new StubInventoryTradePort(),
            new StubCatalogReader(),
            new FixedTimeProvider() );
        Guid operationId = Guid.NewGuid();

        PurchaseReservationDto reservation = await service.ReserveAsync(
            7,
            operationId,
            state.Offer.OfferKey,
            21,
            2,
            11,
            CancellationToken.None );

        Assert.Equal( 200, state.Wallet.ReservedCopper );
        Assert.Equal( 2, state.Offer.ReservedQuantity );

        PurchaseReservationDto cancelled = await service.CancelAsync(
            7,
            reservation.ReservationKey,
            Guid.NewGuid(),
            11,
            CancellationToken.None );

        Assert.Equal( 0, state.Wallet.ReservedCopper );
        Assert.Equal( 0, state.Offer.ReservedQuantity );
        Assert.Equal(
            Pathfinder.Commerce.Domain.Transactions.PurchaseReservationStatus.Cancelled,
            cancelled.Status );
    }

    [Fact]
    public async Task RepeatedOperationDoesNotDoubleReserve()
    {
        TestState state = await TestState.CreateAsync();
        PurchaseReservationService service = new PurchaseReservationService(
            new PurchaseReservationRepository( state.DbContext ),
            new StubBuyerAccessPolicy(),
            new StubInventoryTradePort(),
            new StubCatalogReader(),
            new FixedTimeProvider() );
        Guid operationId = Guid.NewGuid();

        await service.ReserveAsync(
            7,
            operationId,
            state.Offer.OfferKey,
            21,
            1,
            11,
            CancellationToken.None );
        await service.ReserveAsync(
            7,
            operationId,
            state.Offer.OfferKey,
            21,
            1,
            11,
            CancellationToken.None );

        Assert.Equal( 100, state.Wallet.ReservedCopper );
        Assert.Equal( 1, state.Offer.ReservedQuantity );
        Assert.Single( state.DbContext.PurchaseReservations );
    }

    [Fact]
    public async Task CompletionTransfersCapturedPriceFromReservedFunds()
    {
        TestState state = await TestState.CreateAsync();
        StubInventoryTradePort inventory = new StubInventoryTradePort();
        PurchaseReservationService service = new PurchaseReservationService(
            new PurchaseReservationRepository( state.DbContext ),
            new StubBuyerAccessPolicy(),
            inventory,
            new StubCatalogReader(),
            new FixedTimeProvider() );
        PurchaseReservationDto reservation = await service.ReserveAsync(
            7,
            Guid.NewGuid(),
            state.Offer.OfferKey,
            21,
            2,
            11,
            CancellationToken.None );

        PurchaseReservationDto completed = await service.CompleteAsync(
            7,
            reservation.ReservationKey,
            Guid.NewGuid(),
            11,
            CancellationToken.None );

        Assert.Equal( 800, state.Wallet.BalanceCopper );
        Assert.Equal( 0, state.Wallet.ReservedCopper );
        Assert.Equal( 3, state.Offer.AvailableQuantity );
        Assert.Equal(
            Pathfinder.Commerce.Domain.Transactions.PurchaseReservationStatus.Completed,
            completed.Status );
        Assert.True( inventory.PurchaseCalled );
    }

    [Fact]
    public async Task SaleMovesItemAndCreditsHalfBasePrice()
    {
        TestState state = await TestState.CreateAsync();
        StubInventoryTradePort inventory = new StubInventoryTradePort
        {
            SellableItem = new CommerceSellableItem(
                Guid.NewGuid(),
                7,
                21,
                19,
                1,
                0,
                true ),
        };
        PurchaseReservationService service = new PurchaseReservationService(
            new PurchaseReservationRepository( state.DbContext ),
            new StubBuyerAccessPolicy(),
            inventory,
            new StubCatalogReader(),
            new FixedTimeProvider() );

        ShopSaleDto sale = await service.SellAsync(
            7,
            state.Offer.ShopId,
            21,
            inventory.SellableItem.ItemInstanceKey,
            Guid.NewGuid(),
            11,
            CancellationToken.None );

        Assert.Equal( 500, sale.TotalPriceCopper );
        Assert.Equal( 1500, state.Wallet.BalanceCopper );
        Assert.True( inventory.SaleCalled );
    }

    private sealed class TestState
    {
        private static readonly DateTimeOffset _now =
            new DateTimeOffset( 2026, 7, 23, 11, 0, 0, TimeSpan.Zero );

        private TestState(
            CommerceDbContext dbContext,
            ShopOffer offer,
            Wallet wallet )
        {
            DbContext = dbContext;
            Offer = offer;
            Wallet = wallet;
        }

        public CommerceDbContext DbContext { get; }
        public ShopOffer Offer { get; }
        public Wallet Wallet { get; }

        public static async Task<TestState> CreateAsync()
        {
            DbContextOptions<CommerceDbContext> options =
                new DbContextOptionsBuilder<CommerceDbContext>()
                    .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                    .Options;
            CommerceDbContext dbContext = new CommerceDbContext( options );
            Settlement settlement = Settlement.Create(
                7,
                "Otari",
                4,
                String.Empty,
                String.Empty,
                _now );
            Shop shop = settlement.AddShop( "Market", "General", 4, _now );
            dbContext.Settlements.Add( settlement );
            await dbContext.SaveChangesAsync();
            ShopOffer offer = ShopOffer.CreateCatalog( 7, shop.Id, 19, 5, 100, _now );
            Wallet wallet = Wallet.Create( 7, 21, _now );
            wallet.ApplyAdjustment( Guid.NewGuid(), 1000, "Starting funds", 11, _now );
            dbContext.ShopOffers.Add( offer );
            dbContext.Wallets.Add( wallet );
            await dbContext.SaveChangesAsync();
            return new TestState( dbContext, offer, wallet );
        }
    }

    private sealed class StubBuyerAccessPolicy : ICommerceBuyerAccessPolicy
    {
        public Task<bool> ControlsCharacterAsync(
            int campaignId,
            int actingUserId,
            int characterId,
            CancellationToken cancellationToken ) => Task.FromResult( true );
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() =>
            new DateTimeOffset( 2026, 7, 23, 11, 0, 0, TimeSpan.Zero );
    }

    private sealed class StubCatalogReader : ICommerceCatalogReader
    {
        public Task<bool> IsPublishedConfigurationAsync(
            int itemConfigurationId,
            int campaignId,
            CancellationToken cancellationToken ) => Task.FromResult( true );

        public Task<long?> GetBasePriceCopperAsync(
            int itemConfigurationId,
            int campaignId,
            CancellationToken cancellationToken ) => Task.FromResult<long?>( 1000 );
    }

    private sealed class StubInventoryTradePort : ICommerceInventoryTradePort
    {
        public bool PurchaseCalled { get; private set; }
        public bool SaleCalled { get; private set; }
        public CommerceSellableItem SellableItem { get; init; } = new CommerceSellableItem(
            Guid.NewGuid(),
            7,
            21,
            19,
            1,
            0,
            true );

        public Task<Guid> CompletePurchaseAsync(
            int campaignId,
            int shopId,
            int buyerCharacterId,
            ShopOfferKind offerKind,
            int? itemConfigurationId,
            Guid? itemInstanceKey,
            int quantity,
            Guid operationId,
            int actingUserId,
            DateTimeOffset occurredAtUtc,
            CancellationToken cancellationToken )
        {
            PurchaseCalled = true;
            return Task.FromResult( operationId );
        }

        public Task<CommerceSellableItem?> GetSellableItemAsync(
            int campaignId,
            int characterId,
            Guid itemInstanceKey,
            Guid operationId,
            int actingUserId,
            CancellationToken cancellationToken ) => Task.FromResult<CommerceSellableItem?>(
            SellableItem.ItemInstanceKey == itemInstanceKey ? SellableItem : null );

        public Task MoveSaleToShopAsync(
            int campaignId,
            int shopId,
            CommerceSellableItem item,
            Guid operationId,
            int actingUserId,
            DateTimeOffset occurredAtUtc,
            CancellationToken cancellationToken )
        {
            SaleCalled = true;
            return Task.CompletedTask;
        }
    }
}
