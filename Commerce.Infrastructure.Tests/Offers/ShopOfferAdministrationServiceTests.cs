using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Offers;

namespace Pathfinder.Commerce.Infrastructure.Tests.Offers;

public sealed class ShopOfferAdministrationServiceTests
{
    [Fact]
    public async Task CreatesCatalogOfferForPublishedCampaignConfiguration()
    {
        TestContext test = await CreateContextAsync();
        ShopOfferAdministrationService service = test.CreateService(
            new StubCatalogReader( true ),
            new StubInventoryReader( null ) );

        ShopOfferDto offer = await service.CreateCatalogOfferAsync(
            7,
            test.Shop.Id,
            19,
            3,
            250,
            11,
            CancellationToken.None );

        Assert.Equal( 19, offer.ItemConfigurationId );
        Assert.True( test.Inventory.EnsureCalled );
    }

    [Fact]
    public async Task RejectsStockItemFromAnotherCampaign()
    {
        TestContext test = await CreateContextAsync();
        Guid itemKey = Guid.NewGuid();
        ShopOfferAdministrationService service = test.CreateService(
            new StubCatalogReader( true ),
            new StubInventoryReader(
                new CommerceStockItem( itemKey, 8, test.Shop.Id, 1, true ) ) );

        CommerceException exception = await Assert.ThrowsAsync<CommerceException>(
            () => service.CreateStockInstanceOfferAsync(
                7,
                test.Shop.Id,
                itemKey,
                1,
                250,
                11,
                CancellationToken.None ) );

        Assert.Contains( "does not belong", exception.Message );
    }

    private static async Task<TestContext> CreateContextAsync()
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
            TestContext.Now );
        Shop shop = settlement.AddShop( "Market", "General", 4, TestContext.Now );
        dbContext.Settlements.Add( settlement );
        await dbContext.SaveChangesAsync();
        return new TestContext( dbContext, shop );
    }

    private sealed class TestContext
    {
        public static readonly DateTimeOffset Now =
            new DateTimeOffset( 2026, 7, 23, 9, 0, 0, TimeSpan.Zero );

        public TestContext( CommerceDbContext dbContext, Shop shop )
        {
            DbContext = dbContext;
            Shop = shop;
            Inventory = new StubInventoryReader( null );
        }

        public CommerceDbContext DbContext { get; }
        public Shop Shop { get; }
        public StubInventoryReader Inventory { get; private set; }

        public ShopOfferAdministrationService CreateService(
            ICommerceCatalogReader catalog,
            StubInventoryReader inventory )
        {
            Inventory = inventory;
            return new ShopOfferAdministrationService(
                new ShopOfferRepository( DbContext ),
                new StubAccessPolicy(),
                catalog,
                inventory,
                new FixedTimeProvider() );
        }
    }

    private sealed class StubAccessPolicy : ICommerceCampaignAccessPolicy
    {
        public Task<bool> IsGameMasterAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken ) => Task.FromResult( true );
    }

    private sealed class StubCatalogReader : ICommerceCatalogReader
    {
        private readonly bool _isPublished;

        public StubCatalogReader( bool isPublished )
        {
            _isPublished = isPublished;
        }

        public Task<bool> IsPublishedConfigurationAsync(
            int itemConfigurationId,
            int campaignId,
            CancellationToken cancellationToken ) => Task.FromResult( _isPublished );
    }

    private sealed class StubInventoryReader : ICommerceInventoryReader
    {
        private readonly CommerceStockItem? _item;

        public StubInventoryReader( CommerceStockItem? item )
        {
            _item = item;
        }

        public bool EnsureCalled { get; private set; }

        public Task EnsureShopContainerAsync(
            int campaignId,
            int shopId,
            CancellationToken cancellationToken )
        {
            EnsureCalled = true;
            return Task.CompletedTask;
        }

        public Task<CommerceStockItem?> GetShopStockAsync(
            Guid itemInstanceKey,
            CancellationToken cancellationToken ) => Task.FromResult( _item );
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => TestContext.Now;
    }
}
