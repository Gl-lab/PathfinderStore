using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Application.Lifecycle;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.Inventory.Infrastructure.Lifecycle;
using Pathfinder.Inventory.Infrastructure.Transfers;

namespace Pathfinder.Inventory.Infrastructure.Tests;

public sealed class InventoryLifecyclePortTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 27, 22, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task PortMutatesItemWithVersionAndOperationId()
    {
        await using InventoryDbContext dbContext = CreateContext();
        InventoryContainer container = CreateContainer();
        ItemInstance item = ItemInstance.CreateConsumableStack(
            Guid.NewGuid(),
            17,
            23,
            2,
            ItemConsumptionMode.ConsumeAmmunition,
            1,
            container,
            null,
            _createdAtUtc );
        dbContext.Containers.Add( container );
        dbContext.ItemInstances.Add( item );
        await dbContext.SaveChangesAsync();
        InventoryLifecyclePort port = CreatePort( dbContext );
        Guid operationId = Guid.NewGuid();
        InventoryLifecycleMutation mutation = new InventoryLifecycleMutation(
            17,
            item.InstanceKey,
            1,
            0,
            operationId );

        ItemLifecycleDto first = await port.ConsumeItemAsync(
            mutation,
            CancellationToken.None );
        ItemLifecycleDto replay = await port.ConsumeItemAsync(
            mutation,
            CancellationToken.None );

        Assert.Equal( 1, first.Quantity );
        Assert.Equal( first, replay );
        Assert.Single( item.Operations );
    }

    [Fact]
    public async Task PortRejectsCrossCampaignMutation()
    {
        await using InventoryDbContext dbContext = CreateContext();
        InventoryContainer container = CreateContainer();
        ItemInstance item = ItemInstance.CreateDurable(
            Guid.NewGuid(),
            17,
            23,
            5,
            20,
            10,
            container,
            null,
            _createdAtUtc );
        dbContext.Containers.Add( container );
        dbContext.ItemInstances.Add( item );
        await dbContext.SaveChangesAsync();
        InventoryLifecyclePort port = CreatePort( dbContext );

        await Assert.ThrowsAsync<InventoryException>( () => port.DamageItemAsync(
            new InventoryLifecycleMutation(
                18,
                item.InstanceKey,
                10,
                0,
                Guid.NewGuid() ),
            CancellationToken.None ) );
        Assert.Equal( 20, item.CurrentHitPoints );
        Assert.Empty( item.Operations );
    }

    [Fact]
    public async Task PortTransfersRuneAtomicallyAcrossThreeItems()
    {
        await using InventoryDbContext dbContext = CreateContext();
        InventoryContainer container = CreateContainer();
        ItemInstance rune = ItemInstance.CreateAttachableRune(
            Guid.NewGuid(),
            17,
            23,
            "rune.potency",
            ItemRuneTargetKind.Weapon,
            container,
            null,
            _createdAtUtc );
        ItemInstance source = CreateTarget( container );
        ItemInstance destination = CreateTarget( container );
        dbContext.Containers.Add( container );
        dbContext.ItemInstances.AddRange( rune, source, destination );
        rune.AttachRuneTo(
            source,
            0,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );
        await dbContext.SaveChangesAsync();
        InventoryLifecyclePort port = CreatePort( dbContext );

        ItemLifecycleDto result = await port.TransferRuneAsync(
            new TransferRuneMutation(
                17,
                rune.InstanceKey,
                source.InstanceKey,
                destination.InstanceKey,
                1,
                1,
                0,
                Guid.NewGuid() ),
            CancellationToken.None );

        Assert.Equal( destination.InstanceKey, result.AttachedToInstanceKey );
        Assert.Equal( 2, source.Version );
        Assert.Equal( 1, destination.Version );
    }

    private static InventoryLifecyclePort CreatePort( InventoryDbContext dbContext )
    {
        return new InventoryLifecyclePort(
            new InventoryTransferRepository( dbContext ),
            new StubTimeProvider() );
    }

    private static InventoryDbContext CreateContext()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new InventoryDbContext( options );
    }

    private static ItemInstance CreateTarget( InventoryContainer container )
    {
        return ItemInstance.CreateRuneCompatible(
            Guid.NewGuid(),
            17,
            24,
            ItemRuneTargetKind.Weapon,
            container,
            null,
            _createdAtUtc );
    }

    private static InventoryContainer CreateContainer()
    {
        return InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            31,
            _createdAtUtc );
    }

    private sealed class StubTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => _createdAtUtc.AddHours( 1 );
    }
}
