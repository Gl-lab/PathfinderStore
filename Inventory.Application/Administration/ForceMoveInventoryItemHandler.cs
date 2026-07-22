using MediatR;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Application.Administration;

public sealed class ForceMoveInventoryItemHandler
    : IRequestHandler<ForceMoveInventoryItemCommand, ForcedInventoryMoveDto>
{
    private readonly IInventoryTransferRepository _repository;
    private readonly IInventoryGameMasterAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public ForceMoveInventoryItemHandler(
        IInventoryTransferRepository repository,
        IInventoryGameMasterAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<ForcedInventoryMoveDto> Handle(
        ForceMoveInventoryItemCommand request,
        CancellationToken cancellationToken )
    {
        bool isGameMaster = await _accessPolicy.IsGameMasterAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        if ( !isGameMaster )
        {
            throw new InventoryException(
                "Only an active campaign Game Master can force-move an item." );
        }

        InventoryAuditEntry? replay = await _repository.GetAuditAsync(
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.ForcedMove,
            cancellationToken );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                request.CampaignId,
                request.OperationId,
                InventoryAuditActionKind.ForcedMove,
                request.ActingUserId,
                true,
                request.Reason,
                request.ItemInstanceKey,
                request.DestinationContainerKey );
            ItemInstance replayItem = await GetItemAsync(
                request.ItemInstanceKey,
                cancellationToken );
            return new ForcedInventoryMoveDto(
                replayItem.InstanceKey,
                replayItem.CurrentContainerKey,
                replayItem.Version,
                replay.AuditKey );
        }

        InventoryContainer destination = await _repository.GetContainerByKeyAsync(
            request.CampaignId,
            request.DestinationContainerKey,
            cancellationToken ) ?? throw new InventoryException( "Destination container was not found." );
        ItemInstance item = await GetItemAsync( request.ItemInstanceKey, cancellationToken );
        DateTimeOffset occurredAtUtc = _timeProvider.GetUtcNow();
        item.ForceMoveTo(
            destination,
            request.Reason,
            request.ExpectedItemVersion,
            request.OperationId,
            $"gm:{request.ActingUserId}",
            occurredAtUtc );
        InventoryAuditEntry audit = InventoryAuditEntry.Create(
            request.OperationId,
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.ForcedMove,
            request.ActingUserId,
            true,
            request.Reason,
            request.ItemInstanceKey,
            request.DestinationContainerKey,
            occurredAtUtc );
        _repository.AddAudit( audit );
        await _repository.SaveChangesAsync( cancellationToken );
        return new ForcedInventoryMoveDto(
            item.InstanceKey,
            item.CurrentContainerKey,
            item.Version,
            audit.AuditKey );
    }

    private async Task<ItemInstance> GetItemAsync(
        Guid itemInstanceKey,
        CancellationToken cancellationToken )
    {
        return await _repository.GetItemAsync(
            itemInstanceKey,
            cancellationToken ) ?? throw new InventoryException( "Item was not found." );
    }
}
