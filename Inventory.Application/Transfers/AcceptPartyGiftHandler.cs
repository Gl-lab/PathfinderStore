using MediatR;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Application.Audit;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed class AcceptPartyGiftHandler
    : IRequestHandler<AcceptPartyGiftCommand, PartyGiftDto>
{
    private readonly IInventoryTransferRepository _repository;
    private readonly IPartyTransferAccessPolicy _accessPolicy;
    private readonly IItemTransferRestrictionPolicy _restrictionPolicy;
    private readonly TimeProvider _timeProvider;

    public AcceptPartyGiftHandler(
        IInventoryTransferRepository repository,
        IPartyTransferAccessPolicy accessPolicy,
        IItemTransferRestrictionPolicy restrictionPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _restrictionPolicy = restrictionPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<PartyGiftDto> Handle(
        AcceptPartyGiftCommand request,
        CancellationToken cancellationToken )
    {
        PartyGift gift = await _repository.GetGiftAsync(
            request.GiftKey,
            cancellationToken ) ?? throw new InventoryException( "Gift was not found." );
        if ( gift.CampaignId != request.CampaignId )
        {
            throw new InventoryException( "Gift does not belong to this campaign." );
        }

        PartyTransferAccess access = await _accessPolicy.GetAccessAsync(
            gift.CampaignId,
            request.ActingUserId,
            gift.SourceCharacterId,
            gift.DestinationCharacterId,
            cancellationToken );
        if ( !access.SameActiveParty ||
             ( access.PartyId != gift.PartyId ) ||
             !access.ControlsDestination )
        {
            throw new InventoryException(
                "Only the controlling player of the recipient in the same active party can accept this gift." );
        }

        if ( gift.Status == PartyGiftStatus.Accepted )
        {
            gift.Accept( gift.ExpectedItemVersion, request.OperationId, gift.AcceptedAtUtc!.Value );
            return gift.ToDto();
        }

        InventoryContainer sourceContainer = await _repository.GetCharacterContainerAsync(
            gift.CampaignId,
            gift.SourceCharacterId,
            cancellationToken ) ?? throw new InventoryException( "Source character inventory was not found." );
        InventoryContainer destinationContainer = await _repository.GetCharacterContainerAsync(
            gift.CampaignId,
            gift.DestinationCharacterId,
            cancellationToken ) ?? throw new InventoryException( "Recipient character inventory was not found." );
        ItemInstance item = await _repository.GetItemAsync(
            gift.ItemInstanceKey,
            cancellationToken ) ?? throw new InventoryException( "Gift item was not found." );
        if ( ( item.CurrentContainerKey != sourceContainer.ContainerKey ) ||
             item.IsDepleted ||
             ( item.ReservationKey is not null ) )
        {
            throw new InventoryException( "Gift item is no longer available from the source character." );
        }

        bool isEquipped = await _restrictionPolicy.IsEquippedAsync(
            gift.SourceCharacterId,
            gift.ItemInstanceKey,
            cancellationToken );
        if ( isEquipped )
        {
            throw new InventoryException( "An equipped item cannot be gifted." );
        }

        DateTimeOffset acceptedAtUtc = _timeProvider.GetUtcNow();
        gift.Accept( item.Version, request.OperationId, acceptedAtUtc );
        item.MoveTo(
            destinationContainer,
            "party-gift",
            gift.ExpectedItemVersion,
            request.OperationId,
            $"user:{request.ActingUserId}",
            acceptedAtUtc );
        _repository.AddAudit( InventoryAuditFactory.CreatePlayerAction(
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.GiftAccepted,
            request.ActingUserId,
            "Party gift accepted.",
            gift.ItemInstanceKey,
            gift.GiftKey,
            acceptedAtUtc ) );
        await _repository.SaveChangesAsync( cancellationToken );
        return gift.ToDto();
    }
}
