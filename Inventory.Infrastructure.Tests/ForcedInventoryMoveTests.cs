using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Application.Administration;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.Inventory.Infrastructure.Transfers;

namespace Pathfinder.Inventory.Infrastructure.Tests;

public sealed class ForcedInventoryMoveTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new( 2026, 7, 22, 21, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task GameMasterForceMoveClearsReservationAndWritesForcedAudit()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using InventoryDbContext context = new( options );
        InventoryContainer source = CreateContainer( 31 );
        InventoryContainer destination = CreateContainer( 32 );
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            source,
            null,
            _createdAtUtc );
        item.Reserve(
            Guid.NewGuid(),
            0,
            Guid.NewGuid(),
            _createdAtUtc.AddMinutes( 1 ) );
        context.AddRange( source, destination, item );
        await context.SaveChangesAsync();
        ForceMoveInventoryItemHandler handler = new(
            new InventoryTransferRepository( context ),
            new StubGameMasterAccessPolicy( true ),
            new StubTimeProvider() );

        ForcedInventoryMoveDto result = await handler.Handle(
            new ForceMoveInventoryItemCommand(
                101,
                17,
                item.InstanceKey,
                destination.ContainerKey,
                1,
                Guid.NewGuid(),
                "Correct mistaken ownership" ),
            CancellationToken.None );

        Assert.Equal( destination.ContainerKey, result.DestinationContainerKey );
        Assert.Null( item.ReservationKey );
        Assert.Equal( 2, item.Version );
        InventoryAuditEntry audit = await context.AuditEntries.SingleAsync();
        Assert.True( audit.IsForced );
        Assert.Equal( InventoryAuditActionKind.ForcedMove, audit.ActionKind );
        Assert.Equal( "Correct mistaken ownership", audit.Reason );
    }

    [Fact]
    public async Task OrdinaryMemberCannotForceMove()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using InventoryDbContext context = new( options );
        ForceMoveInventoryItemHandler handler = new(
            new InventoryTransferRepository( context ),
            new StubGameMasterAccessPolicy( false ),
            new StubTimeProvider() );

        await Assert.ThrowsAsync<InventoryException>( () => handler.Handle(
            new ForceMoveInventoryItemCommand(
                101,
                17,
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                Guid.NewGuid(),
                "Unauthorized" ),
            CancellationToken.None ) );
    }

    private static InventoryContainer CreateContainer( int characterId ) =>
        InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            characterId,
            _createdAtUtc );

    private sealed class StubGameMasterAccessPolicy : IInventoryGameMasterAccessPolicy
    {
        private readonly bool _isGameMaster;

        public StubGameMasterAccessPolicy( bool isGameMaster )
        {
            _isGameMaster = isGameMaster;
        }

        public Task<bool> IsGameMasterAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken ) => Task.FromResult( _isGameMaster );
    }

    private sealed class StubTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => _createdAtUtc.AddHours( 1 );
    }
}
