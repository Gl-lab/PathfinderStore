using MediatR;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;

namespace Pathfinder.Inventory.Application.Administration;

public sealed class CorrectItemTransferRestrictionHandler
    : IRequestHandler<
        CorrectItemTransferRestrictionCommand,
        CorrectedItemTransferRestrictionDto>
{
    private static readonly Guid _restrictedStateKey =
        new Guid( "11111111-1111-1111-1111-111111111111" );
    private static readonly Guid _unrestrictedStateKey =
        new Guid( "22222222-2222-2222-2222-222222222222" );

    private readonly IInventoryTransferRepository _repository;
    private readonly IInventoryGameMasterAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public CorrectItemTransferRestrictionHandler(
        IInventoryTransferRepository repository,
        IInventoryGameMasterAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<CorrectedItemTransferRestrictionDto> Handle(
        CorrectItemTransferRestrictionCommand request,
        CancellationToken cancellationToken )
    {
        bool isGameMaster = await _accessPolicy.IsGameMasterAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        if ( !isGameMaster )
        {
            throw new InventoryException(
                "Only an active campaign Game Master can correct an item." );
        }

        InventoryAuditEntry? replay = await _repository.GetAuditAsync(
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.ForcedCorrection,
            cancellationToken );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                request.CampaignId,
                request.OperationId,
                InventoryAuditActionKind.ForcedCorrection,
                request.ActingUserId,
                true,
                request.Reason,
                request.ItemInstanceKey,
                GetStateKey( request.IsTransferRestricted ) );
            ItemInstance replayItem = await GetItemAsync(
                request.ItemInstanceKey,
                request.CampaignId,
                cancellationToken );
            return ToDto( replayItem, replay );
        }

        ItemInstance item = await GetItemAsync(
            request.ItemInstanceKey,
            request.CampaignId,
            cancellationToken );
        DateTimeOffset occurredAtUtc = _timeProvider.GetUtcNow();
        item.SetTransferRestriction(
            request.IsTransferRestricted,
            request.ExpectedItemVersion,
            request.OperationId,
            occurredAtUtc );
        InventoryAuditEntry audit = InventoryAuditEntry.Create(
            request.OperationId,
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.ForcedCorrection,
            request.ActingUserId,
            true,
            request.Reason,
            request.ItemInstanceKey,
            GetStateKey( request.IsTransferRestricted ),
            occurredAtUtc );
        _repository.AddAudit( audit );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( item, audit );
    }

    private async Task<ItemInstance> GetItemAsync(
        Guid itemInstanceKey,
        int campaignId,
        CancellationToken cancellationToken )
    {
        ItemInstance item = await _repository.GetItemAsync(
            itemInstanceKey,
            cancellationToken ) ?? throw new InventoryException( "Item was not found." );
        if ( item.CampaignId != campaignId )
        {
            throw new InventoryException( "Item was not found in this campaign." );
        }

        return item;
    }

    private static CorrectedItemTransferRestrictionDto ToDto(
        ItemInstance item,
        InventoryAuditEntry audit )
    {
        return new CorrectedItemTransferRestrictionDto(
            item.InstanceKey,
            item.IsTransferRestricted,
            item.Version,
            audit.AuditKey );
    }

    private static Guid GetStateKey( bool isTransferRestricted ) =>
        isTransferRestricted ? _restrictedStateKey : _unrestrictedStateKey;
}