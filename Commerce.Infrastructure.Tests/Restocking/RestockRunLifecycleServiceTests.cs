using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Offers;
using Pathfinder.Commerce.Infrastructure.Restocking;
using Pathfinder.Commerce.Infrastructure.Shops;

namespace Pathfinder.Commerce.Infrastructure.Tests.Restocking;

public sealed class RestockRunLifecycleServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 27, 8, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task ConfirmationPublishesOffersAndUniqueInstanceExactlyOnce()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CommerceDbContext dbContext = new CommerceDbContext( options );
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
        RestockPolicy policy = RestockPolicy.Create(
            7,
            shop.Id,
            "Weekly",
            2,
            new RestockPolicyConstraints(
                0,
                20,
                1000,
                RestockItemRarity.All,
                RestockItemAccess.All,
                RestockItemCategory.All ),
            new RestockSelectionWeights( 1, 1, 1 ),
            11,
            _now );
        dbContext.RestockPolicies.Add( policy );
        await dbContext.SaveChangesAsync();
        RestockRun run = RestockRun.CreatePreview(
            7,
            shop.Id,
            policy.Id,
            1,
            42,
            11,
            _now,
            [
                new RestockCandidate(
                    10,
                    1,
                    200,
                    RestockItemRarity.Common,
                    RestockItemAccess.Global,
                    RestockItemCategory.Consumable,
                    RestockItemKind.Consumable ),
                new RestockCandidate(
                    20,
                    2,
                    300,
                    RestockItemRarity.Unique,
                    RestockItemAccess.Campaign,
                    RestockItemCategory.OtherEquipment,
                    RestockItemKind.Unique ),
            ] );
        dbContext.RestockRuns.Add( run );
        await dbContext.SaveChangesAsync();
        StubInventoryPort inventoryPort = new StubInventoryPort();
        RestockRunLifecycleService service = new RestockRunLifecycleService(
            new RestockRunRepository( dbContext ),
            new ShopOfferRepository( dbContext ),
            new SettlementRepository( dbContext ),
            new StubAccessPolicy(),
            inventoryPort,
            new FixedTimeProvider() );

        RestockRunDto first = await service.ConfirmAsync(
            7,
            shop.Id,
            run.RunKey,
            11,
            CancellationToken.None );
        RestockRunDto replay = await service.ConfirmAsync(
            7,
            shop.Id,
            run.RunKey,
            11,
            CancellationToken.None );

        Assert.Equal( RestockRunStatus.Confirmed, first.Status );
        Assert.Equal( first.RunKey, replay.RunKey );
        Assert.Equal( first.Lines, replay.Lines );
        Assert.Equal( 2, dbContext.ShopOffers.Count() );
        Assert.Equal( 1, inventoryPort.CreatedCount );
        Assert.Single( first.Lines, line => line.PublishedItemInstanceKey is not null );

        RestockRun rejectedRun = RestockRun.CreatePreview(
            7,
            shop.Id,
            policy.Id,
            1,
            43,
            11,
            _now,
            [
                new RestockCandidate(
                    30,
                    3,
                    400,
                    RestockItemRarity.Unique,
                    RestockItemAccess.Campaign,
                    RestockItemCategory.OtherEquipment,
                    RestockItemKind.Unique ),
            ] );
        dbContext.RestockRuns.Add( rejectedRun );
        await dbContext.SaveChangesAsync();

        RestockRunDto rejected = await service.RejectAsync(
            7,
            shop.Id,
            rejectedRun.RunKey,
            11,
            CancellationToken.None );

        Assert.Equal( RestockRunStatus.Rejected, rejected.Status );
        Assert.Equal( 2, dbContext.ShopOffers.Count() );
        Assert.Equal( 1, inventoryPort.DiscardedCount );
    }

    private sealed class StubAccessPolicy : ICommerceCampaignAccessPolicy
    {
        public Task<bool> IsGameMasterAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken ) => Task.FromResult( true );
    }

    private sealed class StubInventoryPort : ICommerceRestockInventoryPort
    {
        public int CreatedCount { get; private set; }
        public int DiscardedCount { get; private set; }

        public Task EnsureShopContainerAsync(
            int campaignId,
            int shopId,
            CancellationToken cancellationToken )
        {
            DiscardedCount++;
            return Task.CompletedTask;
        }

        public Task<Guid> EnsureUniqueShopStockAsync(
            int campaignId,
            int shopId,
            int itemConfigurationId,
            Guid itemInstanceKey,
            DateTimeOffset createdAtUtc,
            CancellationToken cancellationToken )
        {
            CreatedCount++;
            return Task.FromResult( itemInstanceKey );
        }

        public Task DiscardUniqueShopStockAsync(
            int campaignId,
            int shopId,
            int itemConfigurationId,
            Guid itemInstanceKey,
            CancellationToken cancellationToken ) => Task.CompletedTask;
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() =>
            new DateTimeOffset( 2026, 7, 27, 9, 0, 0, TimeSpan.Zero );
    }
}
