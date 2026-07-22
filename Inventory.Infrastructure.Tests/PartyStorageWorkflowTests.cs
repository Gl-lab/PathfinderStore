using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Application.Storage;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.Inventory.Infrastructure.Transfers;

namespace Pathfinder.Inventory.Infrastructure.Tests;

public sealed class PartyStorageWorkflowTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new( 2026, 7, 22, 20, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task ControlledCharacterDepositsAndWithdrawsUnderFreePolicy()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using InventoryDbContext context = new( options );
        InventoryContainer characterContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            31,
            _createdAtUtc );
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            characterContainer,
            null,
            _createdAtUtc );
        context.AddRange( characterContainer, item );
        await context.SaveChangesAsync();
        InventoryTransferRepository repository = new( context );
        StubStorageAccessPolicy accessPolicy = new(
            PartyStorageWithdrawalPolicy.FreeForMembers,
            controlsCharacter: true,
            isGameMaster: false );
        DepositPartyStorageHandler depositHandler = new(
            repository,
            accessPolicy,
            new StubRestrictionPolicy(),
            new StubTimeProvider( 1 ) );

        PartyStorageItemDto deposited = await depositHandler.Handle(
            new DepositPartyStorageCommand(
                101,
                17,
                31,
                item.InstanceKey,
                0,
                Guid.NewGuid() ),
            CancellationToken.None );
        InventoryContainer partyContainer = await context.Containers.SingleAsync( container =>
            container.OwnerKind == InventoryContainerOwnerKind.Party );

        Assert.Equal( partyContainer.ContainerKey, deposited.ContainerKey );
        Assert.Equal( 41, partyContainer.OwnerId );
        WithdrawPartyStorageHandler withdrawHandler = new(
            repository,
            accessPolicy,
            new StubRestrictionPolicy(),
            new StubTimeProvider( 2 ) );
        PartyStorageItemDto withdrawn = await withdrawHandler.Handle(
            new WithdrawPartyStorageCommand(
                101,
                17,
                31,
                item.InstanceKey,
                1,
                Guid.NewGuid() ),
            CancellationToken.None );

        Assert.Equal( characterContainer.ContainerKey, withdrawn.ContainerKey );
        Assert.Equal( 2, item.Version );
        Assert.Equal( 2, await context.AuditEntries.CountAsync() );
    }

    [Fact]
    public async Task GameMasterPolicyRejectsOrdinaryPlayerWithdrawal()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using InventoryDbContext context = new( options );
        InventoryContainer characterContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Character,
            31,
            _createdAtUtc );
        InventoryContainer partyContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            17,
            InventoryContainerOwnerKind.Party,
            41,
            _createdAtUtc );
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            17,
            23,
            partyContainer,
            null,
            _createdAtUtc );
        context.AddRange( characterContainer, partyContainer, item );
        await context.SaveChangesAsync();
        WithdrawPartyStorageHandler handler = new(
            new InventoryTransferRepository( context ),
            new StubStorageAccessPolicy(
                PartyStorageWithdrawalPolicy.GameMasterOnly,
                controlsCharacter: true,
                isGameMaster: false ),
            new StubRestrictionPolicy(),
            new StubTimeProvider( 1 ) );

        await Assert.ThrowsAsync<InventoryException>( () => handler.Handle(
            new WithdrawPartyStorageCommand(
                101,
                17,
                31,
                item.InstanceKey,
                0,
                Guid.NewGuid() ),
            CancellationToken.None ) );
    }

    private sealed class StubStorageAccessPolicy : IPartyStorageAccessPolicy
    {
        private readonly PartyStorageWithdrawalPolicy _policy;
        private readonly bool _controlsCharacter;
        private readonly bool _isGameMaster;

        public StubStorageAccessPolicy(
            PartyStorageWithdrawalPolicy policy,
            bool controlsCharacter,
            bool isGameMaster )
        {
            _policy = policy;
            _controlsCharacter = controlsCharacter;
            _isGameMaster = isGameMaster;
        }

        public Task<PartyStorageAccess> GetAccessAsync(
            int campaignId,
            int actingUserId,
            int characterId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( new PartyStorageAccess(
                true,
                41,
                _controlsCharacter,
                _isGameMaster,
                _policy ) );
        }
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

        public StubTimeProvider( int hours )
        {
            _hours = hours;
        }

        public override DateTimeOffset GetUtcNow() => _createdAtUtc.AddHours( _hours );
    }
}
