using MediatR;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed class CreatePartyGiftHandler
    : IRequestHandler<CreatePartyGiftCommand, PartyGiftDto>
{
    private static readonly TimeSpan _giftLifetime = TimeSpan.FromDays( 7 );
    private readonly IInventoryTransferRepository _repository;
    private readonly IPartyTransferAccessPolicy _accessPolicy;
    private readonly IItemTransferRestrictionPolicy _restrictionPolicy;
    private readonly TimeProvider _timeProvider;

    public CreatePartyGiftHandler(
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
        CreatePartyGiftCommand request,
        CancellationToken cancellationToken )
    {
        PartyTransferAccess access = await _accessPolicy.GetAccessAsync(
            request.CampaignId,
            request.ActingUserId,
            request.SourceCharacterId,
            request.DestinationCharacterId,
            cancellationToken );
        if ( !access.SameActiveParty || !access.ControlsSource )
        {
            throw new InventoryException(
                "Gift source and recipient must belong to the same active party, and the actor must control the source character." );
        }

        PartyGift? replay = await _repository.GetGiftAsync( request.GiftKey, cancellationToken );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                request.CampaignId,
                request.SourceCharacterId,
                request.DestinationCharacterId,
                request.ItemInstanceKey,
                request.ExpectedItemVersion );
            return replay.ToDto();
        }

        InventoryContainer sourceContainer = await _repository.GetCharacterContainerAsync(
            request.CampaignId,
            request.SourceCharacterId,
            cancellationToken ) ?? throw new InventoryException( "Source character inventory was not found." );
        ItemInstance item = await _repository.GetItemAsync(
            request.ItemInstanceKey,
            cancellationToken ) ?? throw new InventoryException( "Gift item was not found." );
        if ( ( item.CampaignId != request.CampaignId ) ||
             ( item.CurrentContainerKey != sourceContainer.ContainerKey ) ||
             item.IsDepleted ||
             ( item.ReservationKey is not null ) ||
             ( item.Version != request.ExpectedItemVersion ) )
        {
            throw new InventoryException(
                "Gift item is unavailable, not owned by the source character, or has changed." );
        }

        bool isEquipped = await _restrictionPolicy.IsEquippedAsync(
            request.SourceCharacterId,
            request.ItemInstanceKey,
            cancellationToken );
        if ( isEquipped )
        {
            throw new InventoryException( "An equipped item cannot be gifted." );
        }

        DateTimeOffset createdAtUtc = _timeProvider.GetUtcNow();
        PartyGift gift = PartyGift.Create(
            request.GiftKey,
            request.CampaignId,
            access.PartyId,
            request.SourceCharacterId,
            request.DestinationCharacterId,
            request.ItemInstanceKey,
            request.ExpectedItemVersion,
            createdAtUtc,
            createdAtUtc.Add( _giftLifetime ) );
        _repository.AddGift( gift );
        await _repository.SaveChangesAsync( cancellationToken );
        return gift.ToDto();
    }
}
