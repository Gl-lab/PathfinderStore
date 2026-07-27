using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Application.Lifecycle;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.Inventory.Infrastructure.Transfers;

namespace Pathfinder.Inventory.Infrastructure.Tests;

public sealed class InventoryLifecycleCommandTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 27, 21, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task AuthorizedCommandConsumesChargesAndReplaysIdempotently()
    {
        await using InventoryDbContext dbContext = CreateContext();
        InventoryContainer container = CreateContainer( 31 );
        ItemInstance item = ItemInstance.CreateCharged(
            Guid.NewGuid(),
            17,
            23,
            3,
            1,
            ItemChargeRecoveryRule.Manual,
            container,
            null,
            _createdAtUtc );
        dbContext.Containers.Add( container );
        dbContext.ItemInstances.Add( item );
        await dbContext.SaveChangesAsync();
        InventoryLifecycleCommandHandler handler = CreateHandler( dbContext, true );
        Guid operationId = Guid.NewGuid();
        ConsumeItemChargesCommand command = new ConsumeItemChargesCommand(
            101,
            17,
            item.InstanceKey,
            1,
            0,
            operationId );

        ItemLifecycleDto first = await handler.Handle( command, CancellationToken.None );
        ItemLifecycleDto replay = await handler.Handle( command, CancellationToken.None );

        Assert.Equal( 2, first.CurrentCharges );
        Assert.Equal( first, replay );
        Assert.Single( item.Operations );
    }

    [Fact]
    public async Task UnauthorizedCommandDoesNotMutateItem()
    {
        await using InventoryDbContext dbContext = CreateContext();
        InventoryContainer container = CreateContainer( 31 );
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
        InventoryLifecycleCommandHandler handler = CreateHandler( dbContext, false );

        await Assert.ThrowsAsync<InventoryException>( () => handler.Handle(
            new DamageInventoryItemCommand(
                101,
                17,
                item.InstanceKey,
                10,
                0,
                Guid.NewGuid() ),
            CancellationToken.None ) );
        Assert.Equal( 20, item.CurrentHitPoints );
        Assert.Empty( item.Operations );
    }

    [Fact]
    public async Task RuneCommandRequiresAccessToBothAggregates()
    {
        await using InventoryDbContext dbContext = CreateContext();
        InventoryContainer runeContainer = CreateContainer( 31 );
        InventoryContainer targetContainer = CreateContainer( 32 );
        ItemInstance rune = ItemInstance.CreateAttachableRune(
            Guid.NewGuid(),
            17,
            23,
            "rune.potency",
            ItemRuneTargetKind.Weapon,
            runeContainer,
            null,
            _createdAtUtc );
        ItemInstance target = ItemInstance.CreateRuneCompatible(
            Guid.NewGuid(),
            17,
            24,
            ItemRuneTargetKind.Weapon,
            targetContainer,
            null,
            _createdAtUtc );
        dbContext.Containers.AddRange( runeContainer, targetContainer );
        dbContext.ItemInstances.AddRange( rune, target );
        await dbContext.SaveChangesAsync();
        InventoryLifecycleCommandHandler handler = new InventoryLifecycleCommandHandler(
            new InventoryTransferRepository( dbContext ),
            new OwnerAccessPolicy( 31 ),
            new StubTimeProvider() );

        await Assert.ThrowsAsync<InventoryException>( () => handler.Handle(
            new AttachInventoryRuneCommand(
                101,
                17,
                rune.InstanceKey,
                target.InstanceKey,
                0,
                0,
                Guid.NewGuid() ),
            CancellationToken.None ) );
        Assert.Null( rune.AttachedToInstanceKey );
        Assert.Empty( rune.Operations );
        Assert.Empty( target.Operations );
    }

    [Fact]
    public async Task AttachedRuneAuthorizationUsesCurrentTargetOwner()
    {
        await using InventoryDbContext dbContext = CreateContext();
        InventoryContainer oldRuneContainer = CreateContainer( 31 );
        InventoryContainer targetContainer = CreateContainer( 32 );
        ItemInstance rune = ItemInstance.CreateAttachableRune(
            Guid.NewGuid(),
            17,
            23,
            "rune.potency",
            ItemRuneTargetKind.Weapon,
            oldRuneContainer,
            null,
            _createdAtUtc );
        ItemInstance source = ItemInstance.CreateRuneCompatible(
            Guid.NewGuid(),
            17,
            24,
            ItemRuneTargetKind.Weapon,
            targetContainer,
            null,
            _createdAtUtc );
        ItemInstance destination = ItemInstance.CreateRuneCompatible(
            Guid.NewGuid(),
            17,
            25,
            ItemRuneTargetKind.Weapon,
            targetContainer,
            null,
            _createdAtUtc );
        dbContext.Containers.AddRange( oldRuneContainer, targetContainer );
        dbContext.ItemInstances.AddRange( rune, source, destination );
        rune.AttachRuneTo(
            source,
            0,
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );
        await dbContext.SaveChangesAsync();
        InventoryLifecycleCommandHandler handler = new InventoryLifecycleCommandHandler(
            new InventoryTransferRepository( dbContext ),
            new OwnerAccessPolicy( 32 ),
            new StubTimeProvider() );

        ItemLifecycleDto result = await handler.Handle(
            new TransferInventoryRuneCommand(
                102,
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
    }

    private static InventoryLifecycleCommandHandler CreateHandler(
        InventoryDbContext dbContext,
        bool canMutate )
    {
        return new InventoryLifecycleCommandHandler(
            new InventoryTransferRepository( dbContext ),
            new StubAccessPolicy( canMutate ),
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

    private static InventoryContainer CreateContainer( int ownerId )
    {
        return InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            ownerId,
            _createdAtUtc );
    }

    private sealed class StubAccessPolicy : IInventoryLifecycleAccessPolicy
    {
        private readonly bool _canMutate;

        public StubAccessPolicy( bool canMutate )
        {
            _canMutate = canMutate;
        }

        public Task<bool> CanMutateAsync(
            int campaignId,
            int actingUserId,
            InventoryContainerOwnerKind ownerKind,
            int ownerId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( _canMutate );
        }
    }

    private sealed class OwnerAccessPolicy : IInventoryLifecycleAccessPolicy
    {
        private readonly int _allowedOwnerId;

        public OwnerAccessPolicy( int allowedOwnerId )
        {
            _allowedOwnerId = allowedOwnerId;
        }

        public Task<bool> CanMutateAsync(
            int campaignId,
            int actingUserId,
            InventoryContainerOwnerKind ownerKind,
            int ownerId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( ownerId == _allowedOwnerId );
        }
    }

    private sealed class StubTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => _createdAtUtc.AddHours( 1 );
    }
}
