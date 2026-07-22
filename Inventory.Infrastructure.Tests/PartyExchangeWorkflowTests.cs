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
            new StubAccessPolicy( true, false ),
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

        CompletePartyExchangeHandler completeHandler = new(
            new InventoryTransferRepository( context ),
            new StubAccessPolicy( false, true ),
            new StubRestrictionPolicy(),
            new StubTimeProvider( 2 ) );
        PartyExchangeDto completed = await completeHandler.Handle(
            new CompletePartyExchangeCommand( 102, 17, exchangeKey, Guid.NewGuid() ),
            CancellationToken.None );

        Assert.Equal( PartyExchangeStatus.Completed, completed.Status );
        Assert.Equal( secondContainer.ContainerKey, firstItem.CurrentContainerKey );
        Assert.Equal( firstContainer.ContainerKey, secondItem.CurrentContainerKey );
        Assert.Null( firstItem.ReservationKey );
        Assert.Null( secondItem.ReservationKey );
        Assert.Equal( 2, firstItem.Version );
        Assert.Equal( 2, secondItem.Version );
        Assert.Equal( 2, await context.AuditEntries.CountAsync() );
    }

    [Fact]
    public async Task CancelReleasesAllReservations()
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
        InventoryTransferRepository repository = new( context );
        CreatePartyExchangeHandler createHandler = new(
            repository,
            new StubAccessPolicy( true, false ),
            new StubRestrictionPolicy(),
            new StubTimeProvider() );
        await createHandler.Handle(
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
        CancelPartyExchangeHandler cancelHandler = new(
            repository,
            new StubAccessPolicy( true, false ),
            new StubTimeProvider( 2 ) );

        PartyExchangeDto cancelled = await cancelHandler.Handle(
            new CancelPartyExchangeCommand( 101, 17, exchangeKey, Guid.NewGuid() ),
            CancellationToken.None );

        Assert.Equal( PartyExchangeStatus.Cancelled, cancelled.Status );
        Assert.Null( firstItem.ReservationKey );
        Assert.Null( secondItem.ReservationKey );
        Assert.Equal( 2, firstItem.Version );
        Assert.Equal( 2, secondItem.Version );
        Assert.Equal( 2, await context.AuditEntries.CountAsync() );
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
        private readonly bool _controlsSource;
        private readonly bool _controlsDestination;

        public StubAccessPolicy( bool controlsSource, bool controlsDestination )
        {
            _controlsSource = controlsSource;
            _controlsDestination = controlsDestination;
        }

        public Task<PartyTransferAccess> GetAccessAsync(
            int campaignId,
            int actingUserId,
            int sourceCharacterId,
            int destinationCharacterId,
            CancellationToken cancellationToken ) =>
            Task.FromResult( new PartyTransferAccess(
                true,
                41,
                _controlsSource,
                _controlsDestination ) );
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
        private readonly int _hours;

        public StubTimeProvider( int hours = 1 )
        {
            _hours = hours;
        }

        public override DateTimeOffset GetUtcNow() => _createdAtUtc.AddHours( _hours );
    }
}
