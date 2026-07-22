using MediatR;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Application.Storage;

public sealed class DepositPartyStorageHandler : PartyStorageHandlerBase,
    IRequestHandler<DepositPartyStorageCommand, PartyStorageItemDto>
{
    public DepositPartyStorageHandler(
        IInventoryTransferRepository repository,
        IPartyStorageAccessPolicy accessPolicy,
        IItemTransferRestrictionPolicy restrictionPolicy,
        TimeProvider timeProvider )
        : base( repository, accessPolicy, restrictionPolicy, timeProvider )
    {
    }

    public async Task<PartyStorageItemDto> Handle(
        DepositPartyStorageCommand request,
        CancellationToken cancellationToken )
    {
        PartyStorageAccess access = await _accessPolicy.GetAccessAsync(
            request.CampaignId,
            request.ActingUserId,
            request.CharacterId,
            cancellationToken );
        if ( !access.ActiveParty || !access.ControlsCharacter )
        {
            throw new InventoryException(
                "Only a controlling player in the active party can deposit this item." );
        }

        InventoryContainer source = await GetCharacterContainerAsync(
            request.CampaignId,
            request.CharacterId,
            cancellationToken );
        ItemInstance item = await GetAvailableItemAsync(
            request.ItemInstanceKey,
            request.ExpectedItemVersion,
            cancellationToken );
        if ( item.CurrentContainerKey != source.ContainerKey )
        {
            throw new InventoryException( "Item is not owned by the depositing character." );
        }

        bool isEquipped = await _restrictionPolicy.IsEquippedAsync(
            request.CharacterId,
            request.ItemInstanceKey,
            cancellationToken );
        if ( isEquipped )
        {
            throw new InventoryException( "An equipped item cannot be deposited." );
        }

        DateTimeOffset occurredAtUtc = _timeProvider.GetUtcNow();
        InventoryContainer destination = await GetPartyContainerAsync(
            request.CampaignId,
            access.PartyId,
            true,
            occurredAtUtc,
            cancellationToken );
        item.MoveTo(
            destination,
            "party-storage-deposit",
            request.ExpectedItemVersion,
            request.OperationId,
            $"user:{request.ActingUserId}",
            occurredAtUtc );
        await _repository.SaveChangesAsync( cancellationToken );
        return new PartyStorageItemDto(
            item.InstanceKey,
            item.CurrentContainerKey,
            item.Version );
    }
}
