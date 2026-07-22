using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Application.Storage;

public abstract class PartyStorageHandlerBase
{
    protected readonly IInventoryTransferRepository _repository;
    protected readonly IPartyStorageAccessPolicy _accessPolicy;
    protected readonly IItemTransferRestrictionPolicy _restrictionPolicy;
    protected readonly TimeProvider _timeProvider;

    protected PartyStorageHandlerBase(
        IInventoryTransferRepository repository,
        IPartyStorageAccessPolicy accessPolicy,
        IItemTransferRestrictionPolicy restrictionPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _restrictionPolicy = restrictionPolicy;
        _timeProvider = timeProvider;
    }

    protected async Task<InventoryContainer> GetCharacterContainerAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken )
    {
        return await _repository.GetCharacterContainerAsync(
            campaignId,
            characterId,
            cancellationToken ) ?? throw new InventoryException( "Character inventory was not found." );
    }

    protected async Task<ItemInstance> GetAvailableItemAsync(
        Guid itemInstanceKey,
        int expectedVersion,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await _repository.GetItemAsync(
            itemInstanceKey,
            cancellationToken ) ?? throw new InventoryException( "Storage item was not found." );
        if ( item.IsDepleted ||
             ( item.ReservationKey is not null ) ||
             ( item.Version != expectedVersion ) )
        {
            throw new InventoryException( "Storage item is depleted, reserved, or changed." );
        }

        return item;
    }

    protected async Task<InventoryContainer> GetPartyContainerAsync(
        int campaignId,
        int partyId,
        bool createIfMissing,
        DateTimeOffset createdAtUtc,
        CancellationToken cancellationToken )
    {
        InventoryContainer? container = await GetPartyOwnedContainerAsync(
            campaignId,
            partyId,
            cancellationToken );
        if ( ( container is null ) && createIfMissing )
        {
            container = InventoryContainer.CreateRoot(
                CreatePartyContainerKey( campaignId, partyId ),
                campaignId,
                InventoryContainerOwnerKind.Party,
                partyId,
                createdAtUtc );
            _repository.AddContainer( container );
        }

        return container ?? throw new InventoryException( "Party storage inventory was not found." );
    }

    private async Task<InventoryContainer?> GetPartyOwnedContainerAsync(
        int campaignId,
        int partyId,
        CancellationToken cancellationToken )
    {
        return await _repository.GetContainerAsync(
            campaignId,
            InventoryContainerOwnerKind.Party,
            partyId,
            cancellationToken );
    }

    private static Guid CreatePartyContainerKey( int campaignId, int partyId )
    {
        byte[] bytes = new byte[ 16 ];
        bytes[ 0 ] = 0x50;
        bytes[ 1 ] = 0x41;
        bytes[ 2 ] = 0x52;
        bytes[ 3 ] = 0x54;
        BitConverter.GetBytes( campaignId ).CopyTo( bytes, 8 );
        BitConverter.GetBytes( partyId ).CopyTo( bytes, 12 );
        return new Guid( bytes );
    }
}
