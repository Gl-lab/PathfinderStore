using MediatR;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed class CreatePartyExchangeHandler
    : IRequestHandler<CreatePartyExchangeCommand, PartyExchangeDto>
{
    private static readonly TimeSpan _exchangeLifetime = TimeSpan.FromDays( 7 );
    private readonly IInventoryTransferRepository _repository;
    private readonly IPartyTransferAccessPolicy _accessPolicy;
    private readonly IItemTransferRestrictionPolicy _restrictionPolicy;
    private readonly TimeProvider _timeProvider;

    public CreatePartyExchangeHandler(
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

    public async Task<PartyExchangeDto> Handle(
        CreatePartyExchangeCommand request,
        CancellationToken cancellationToken )
    {
        PartyTransferAccess access = await _accessPolicy.GetAccessAsync(
            request.CampaignId,
            request.ActingUserId,
            request.InitiatorCharacterId,
            request.CounterpartyCharacterId,
            cancellationToken );
        if ( !access.SameActiveParty || !access.ControlsSource )
        {
            throw new InventoryException(
                "Exchange characters must belong to the same active party, and the actor must control the initiator." );
        }

        PartyExchange? replay = await _repository.GetExchangeAsync(
            request.ExchangeKey,
            cancellationToken );
        if ( replay is not null )
        {
            replay.EnsureMatches(
                request.CampaignId,
                request.InitiatorCharacterId,
                request.CounterpartyCharacterId,
                request.Lines
                    .Select( line => new PartyExchangeLineRequest(
                        line.FromCharacterId,
                        line.ItemInstanceKey,
                        line.ExpectedItemVersion ) )
                    .ToArray() );
            return replay.ToDto();
        }

        if ( request.Lines.Select( line => line.ReservationOperationId ).Distinct().Count() !=
             request.Lines.Count )
        {
            throw new InventoryException( "Exchange reservation operation ids must be unique." );
        }

        InventoryContainer initiatorContainer = await _repository.GetCharacterContainerAsync(
            request.CampaignId,
            request.InitiatorCharacterId,
            cancellationToken ) ?? throw new InventoryException( "Initiator inventory was not found." );
        InventoryContainer counterpartyContainer = await _repository.GetCharacterContainerAsync(
            request.CampaignId,
            request.CounterpartyCharacterId,
            cancellationToken ) ?? throw new InventoryException( "Counterparty inventory was not found." );
        List<( CreatePartyExchangeLine Line, ItemInstance Item )> resolvedLines = [];
        foreach ( CreatePartyExchangeLine line in request.Lines )
        {
            InventoryContainer expectedContainer = line.FromCharacterId == request.InitiatorCharacterId
                ? initiatorContainer
                : counterpartyContainer;
            ItemInstance item = await _repository.GetItemAsync(
                line.ItemInstanceKey,
                cancellationToken ) ?? throw new InventoryException( "Exchange item was not found." );
            if ( ( item.CampaignId != request.CampaignId ) ||
                 ( item.CurrentContainerKey != expectedContainer.ContainerKey ) ||
                 item.IsDepleted ||
                 ( item.ReservationKey is not null ) ||
                 ( item.Version != line.ExpectedItemVersion ) )
            {
                throw new InventoryException(
                    "Exchange item is unavailable, not owned by its declared character, reserved, or changed." );
            }

            bool isEquipped = await _restrictionPolicy.IsEquippedAsync(
                line.FromCharacterId,
                line.ItemInstanceKey,
                cancellationToken );
            if ( isEquipped )
            {
                throw new InventoryException( "An equipped item cannot be offered for exchange." );
            }

            resolvedLines.Add( ( line, item ) );
        }

        DateTimeOffset createdAtUtc = _timeProvider.GetUtcNow();
        PartyExchange exchange = PartyExchange.Create(
            request.ExchangeKey,
            request.CampaignId,
            access.PartyId,
            request.InitiatorCharacterId,
            request.CounterpartyCharacterId,
            request.Lines
                .Select( line => new PartyExchangeLineRequest(
                    line.FromCharacterId,
                    line.ItemInstanceKey,
                    line.ExpectedItemVersion ) )
                .ToArray(),
            createdAtUtc,
            createdAtUtc.Add( _exchangeLifetime ) );
        foreach ( ( CreatePartyExchangeLine line, ItemInstance item ) in resolvedLines )
        {
            item.Reserve(
                exchange.ExchangeKey,
                line.ExpectedItemVersion,
                line.ReservationOperationId,
                createdAtUtc );
        }

        _repository.AddExchange( exchange );
        await _repository.SaveChangesAsync( cancellationToken );
        return exchange.ToDto();
    }
}
