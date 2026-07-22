using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.Inventory.Infrastructure.Transfers;

namespace Pathfinder.Inventory.Infrastructure.Tests;

public sealed class PartyGiftWorkflowTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new( 2026, 7, 22, 18, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task RecipientAcceptanceMovesItemOnlyAfterConfirmation()
    {
        DbContextOptions<InventoryDbContext> options = CreateOptions();
        Guid itemKey = Guid.NewGuid();
        Guid destinationKey;
        Guid giftKey = Guid.NewGuid();
        Guid operationId = Guid.NewGuid();
        await using ( InventoryDbContext context = new( options ) )
        {
            InventoryContainer source = CreateContainer( 31 );
            InventoryContainer destination = CreateContainer( 32 );
            destinationKey = destination.ContainerKey;
            ItemInstance item = ItemInstance.Create(
                itemKey,
                17,
                23,
                source,
                null,
                _createdAtUtc );
            context.AddRange( source, destination, item );
            await context.SaveChangesAsync();

            InventoryTransferRepository repository = new( context );
            CreatePartyGiftHandler createHandler = new(
                repository,
                new StubAccessPolicy( controlsSource: true, controlsDestination: false ),
                new StubRestrictionPolicy( false ),
                new StubTimeProvider( _createdAtUtc.AddHours( 1 ) ) );
            PartyGiftDto created = await createHandler.Handle(
                new CreatePartyGiftCommand( 101, 17, giftKey, 31, 32, itemKey, 0 ),
                CancellationToken.None );

            Assert.Equal( PartyGiftStatus.Pending, created.Status );
            Assert.Equal( source.ContainerKey, item.CurrentContainerKey );

            AcceptPartyGiftHandler acceptHandler = new(
                repository,
                new StubAccessPolicy( controlsSource: false, controlsDestination: true ),
                new StubRestrictionPolicy( false ),
                new StubTimeProvider( _createdAtUtc.AddHours( 2 ) ) );
            PartyGiftDto accepted = await acceptHandler.Handle(
                new AcceptPartyGiftCommand( 102, 17, giftKey, operationId ),
                CancellationToken.None );

            Assert.Equal( PartyGiftStatus.Accepted, accepted.Status );
            Assert.Equal( destinationKey, item.CurrentContainerKey );
            Assert.Equal( 1, item.Version );
            Assert.Equal( 2, await context.AuditEntries.CountAsync() );

            PartyGiftDto replay = await acceptHandler.Handle(
                new AcceptPartyGiftCommand( 102, 17, giftKey, operationId ),
                CancellationToken.None );
            Assert.Equal( PartyGiftStatus.Accepted, replay.Status );
            Assert.Equal( 1, item.Version );
        }
    }

    [Fact]
    public async Task CreateRejectsEquippedItem()
    {
        DbContextOptions<InventoryDbContext> options = CreateOptions();
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
        context.AddRange( source, destination, item );
        await context.SaveChangesAsync();
        CreatePartyGiftHandler handler = new(
            new InventoryTransferRepository( context ),
            new StubAccessPolicy( controlsSource: true, controlsDestination: false ),
            new StubRestrictionPolicy( true ),
            new StubTimeProvider( _createdAtUtc.AddHours( 1 ) ) );

        await Assert.ThrowsAsync<InventoryException>( () => handler.Handle(
            new CreatePartyGiftCommand(
                101,
                17,
                Guid.NewGuid(),
                31,
                32,
                item.InstanceKey,
                0 ),
            CancellationToken.None ) );
    }

    private static DbContextOptions<InventoryDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;
    }

    private static InventoryContainer CreateContainer( int characterId )
    {
        return InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            characterId,
            _createdAtUtc );
    }

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
            CancellationToken cancellationToken )
        {
            return Task.FromResult( new PartyTransferAccess(
                true,
                41,
                _controlsSource,
                _controlsDestination ) );
        }
    }

    private sealed class StubRestrictionPolicy : IItemTransferRestrictionPolicy
    {
        private readonly bool _isEquipped;

        public StubRestrictionPolicy( bool isEquipped )
        {
            _isEquipped = isEquipped;
        }

        public Task<bool> IsEquippedAsync(
            int characterId,
            Guid itemInstanceKey,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( _isEquipped );
        }
    }

    private sealed class StubTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public StubTimeProvider( DateTimeOffset utcNow )
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
