using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.Inventory.Infrastructure.Transfers;

namespace Pathfinder.Inventory.Infrastructure.Tests;

public sealed class PartyExchangeWorkflowTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new( 2026, 7, 22, 19, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task CreateReservesItemsFromBothCharacters()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using InventoryDbContext context = new( options );
        InventoryContainer firstContainer = CreateContainer( 31 );
        InventoryContainer secondContainer = CreateContainer( 32 );
        ItemInstance firstItem = CreateItem( firstContainer );
        ItemInstance secondItem = CreateItem( secondContainer );
        context.AddRange( firstContainer, secondContainer, firstItem, secondItem );
        await context.SaveChangesAsync();
        Guid exchangeKey = Guid.NewGuid();
        CreatePartyExchangeHandler handler = new(
            new InventoryTransferRepository( context ),
            new StubAccessPolicy(),
            new StubRestrictionPolicy(),
            new StubTimeProvider() );

        PartyExchangeDto exchange = await handler.Handle(
            new CreatePartyExchangeCommand(
                101,
                17,
                exchangeKey,
                31,
                32,
                [
                    new CreatePartyExchangeLine( 31, firstItem.InstanceKey, 0, Guid.NewGuid() ),
                    new CreatePartyExchangeLine( 32, secondItem.InstanceKey, 0, Guid.NewGuid() ),
                ] ),
            CancellationToken.None );

        Assert.Equal( PartyExchangeStatus.Pending, exchange.Status );
        Assert.Equal( exchangeKey, firstItem.ReservationKey );
        Assert.Equal( exchangeKey, secondItem.ReservationKey );
        Assert.Equal( 1, firstItem.Version );
        Assert.Equal( 1, secondItem.Version );
        Assert.Equal( 2, await context.PartyExchanges.SelectMany( item => item.Lines ).CountAsync() );
    }

    private static InventoryContainer CreateContainer( int characterId ) =>
        InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            characterId,
            _createdAtUtc );

    private static ItemInstance CreateItem( InventoryContainer container ) =>
        ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            container,
            null,
            _createdAtUtc );

    private sealed class StubAccessPolicy : IPartyTransferAccessPolicy
    {
        public Task<PartyTransferAccess> GetAccessAsync(
            int campaignId,
            int actingUserId,
            int sourceCharacterId,
            int destinationCharacterId,
            CancellationToken cancellationToken ) =>
            Task.FromResult( new PartyTransferAccess( true, 41, true, false ) );
    }

    private sealed class StubRestrictionPolicy : IItemTransferRestrictionPolicy
    {
        public Task<bool> IsEquippedAsync(
            int characterId,
            Guid itemInstanceKey,
            CancellationToken cancellationToken ) => Task.FromResult( false );
    }

    private sealed class StubTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => _createdAtUtc.AddHours( 1 );
    }
}
