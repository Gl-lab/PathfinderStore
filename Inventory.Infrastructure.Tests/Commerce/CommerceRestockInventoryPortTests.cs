using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Infrastructure.Commerce;
using Pathfinder.Inventory.Infrastructure.Data;

namespace Pathfinder.Inventory.Infrastructure.Tests.Commerce;

public sealed class CommerceRestockInventoryPortTests
{
    [Fact]
    public async Task EnsureUniqueShopStockIsIdempotentForInstanceKey()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using InventoryDbContext dbContext = new InventoryDbContext( options );
        CommerceRestockInventoryPort port = new CommerceRestockInventoryPort(
            dbContext,
            new FixedTimeProvider() );
        Guid instanceKey = Guid.NewGuid();

        await port.EnsureShopContainerAsync( 7, 11, CancellationToken.None );
        Guid first = await port.EnsureUniqueShopStockAsync(
            7,
            11,
            17,
            instanceKey,
            new FixedTimeProvider().GetUtcNow(),
            CancellationToken.None );
        Guid replay = await port.EnsureUniqueShopStockAsync(
            7,
            11,
            17,
            instanceKey,
            new FixedTimeProvider().GetUtcNow(),
            CancellationToken.None );

        Assert.Equal( first, replay );
        Assert.Single( dbContext.ItemInstances );
        Assert.Single( dbContext.Containers );
    }

    [Fact]
    public async Task DiscardRemovesOnlyPreparedRestockInstance()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using InventoryDbContext dbContext = new InventoryDbContext( options );
        CommerceRestockInventoryPort port = new CommerceRestockInventoryPort(
            dbContext,
            new FixedTimeProvider() );
        Guid instanceKey = Guid.NewGuid();
        await port.EnsureShopContainerAsync( 7, 11, CancellationToken.None );
        await port.EnsureUniqueShopStockAsync(
            7,
            11,
            17,
            instanceKey,
            new FixedTimeProvider().GetUtcNow(),
            CancellationToken.None );

        await port.DiscardUniqueShopStockAsync(
            7,
            11,
            17,
            instanceKey,
            CancellationToken.None );

        Assert.Empty( dbContext.ItemInstances );
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() =>
            new DateTimeOffset( 2026, 7, 27, 9, 0, 0, TimeSpan.Zero );
    }
}
