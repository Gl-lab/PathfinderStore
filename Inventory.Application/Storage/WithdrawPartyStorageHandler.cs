using MediatR;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Application.Audit;

namespace Pathfinder.Inventory.Application.Storage;

public sealed class WithdrawPartyStorageHandler : PartyStorageHandlerBase,
    IRequestHandler<WithdrawPartyStorageCommand, PartyStorageItemDto>
{
    public WithdrawPartyStorageHandler(
        IInventoryTransferRepository repository,
        IPartyStorageAccessPolicy accessPolicy,
        IItemTransferRestrictionPolicy restrictionPolicy,
        TimeProvider timeProvider )
        : base( repository, accessPolicy, restrictionPolicy, timeProvider )
    {
    }

    public async Task<PartyStorageItemDto> Handle(
        WithdrawPartyStorageCommand request,
        CancellationToken cancellationToken )
    {
        PartyStorageAccess access = await _accessPolicy.GetAccessAsync(
            request.CampaignId,
            request.ActingUserId,
            request.CharacterId,
            cancellationToken );
        bool permitted = access.WithdrawalPolicy switch
        {
            PartyStorageWithdrawalPolicy.FreeForMembers => access.ControlsCharacter,
            PartyStorageWithdrawalPolicy.GameMasterOnly => access.IsGameMaster,
            _ => false,
        };
        if ( !access.ActiveParty || !permitted )
        {
            throw new InventoryException( "Party storage policy does not permit this withdrawal." );
        }

        InventoryContainer source = await GetPartyContainerAsync(
            request.CampaignId,
            access.PartyId,
            false,
            _timeProvider.GetUtcNow(),
            cancellationToken );
        InventoryContainer destination = await GetCharacterContainerAsync(
            request.CampaignId,
            request.CharacterId,
            cancellationToken );
        ItemInstance item = await GetAvailableItemAsync(
            request.ItemInstanceKey,
            request.ExpectedItemVersion,
            cancellationToken );
        if ( item.CurrentContainerKey != source.ContainerKey )
        {
            throw new InventoryException( "Item does not belong to party storage." );
        }

        DateTimeOffset occurredAtUtc = _timeProvider.GetUtcNow();
        item.MoveTo(
            destination,
            "party-storage-withdrawal",
            request.ExpectedItemVersion,
            request.OperationId,
            $"user:{request.ActingUserId}",
            occurredAtUtc );
        _repository.AddAudit( InventoryAuditFactory.CreatePlayerAction(
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.PartyStorageWithdrawn,
            request.ActingUserId,
            "Item withdrawn from party storage.",
            request.ItemInstanceKey,
            null,
            occurredAtUtc ) );
        await _repository.SaveChangesAsync( cancellationToken );
        return new PartyStorageItemDto(
            item.InstanceKey,
            item.CurrentContainerKey,
            item.Version );
    }
}
