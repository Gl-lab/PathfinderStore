using MediatR;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Application.Audit;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed class CancelPartyExchangeHandler
    : IRequestHandler<CancelPartyExchangeCommand, PartyExchangeDto>
{
    private readonly IInventoryTransferRepository _repository;
    private readonly IPartyTransferAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public CancelPartyExchangeHandler(
        IInventoryTransferRepository repository,
        IPartyTransferAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<PartyExchangeDto> Handle(
        CancelPartyExchangeCommand request,
        CancellationToken cancellationToken )
    {
        PartyExchange exchange = await _repository.GetExchangeAsync(
            request.ExchangeKey,
            cancellationToken ) ?? throw new InventoryException( "Exchange was not found." );
        if ( exchange.CampaignId != request.CampaignId )
        {
            throw new InventoryException( "Exchange does not belong to this campaign." );
        }

        PartyTransferAccess access = await _accessPolicy.GetAccessAsync(
            exchange.CampaignId,
            request.ActingUserId,
            exchange.InitiatorCharacterId,
            exchange.CounterpartyCharacterId,
            cancellationToken );
        if ( !access.SameActiveParty ||
             ( access.PartyId != exchange.PartyId ) ||
             ( !access.ControlsSource && !access.ControlsDestination ) )
        {
            throw new InventoryException( "Only an exchange participant can cancel it." );
        }

        if ( exchange.Status == PartyExchangeStatus.Cancelled )
        {
            exchange.Cancel( request.OperationId, exchange.CancelledAtUtc!.Value );
            return exchange.ToDto();
        }

        List<( PartyExchangeLine Line, ItemInstance Item )> resolvedLines = [];
        foreach ( PartyExchangeLine line in exchange.Lines )
        {
            ItemInstance item = await _repository.GetItemAsync(
                line.ItemInstanceKey,
                cancellationToken ) ?? throw new InventoryException( "Exchange item was not found." );
            if ( ( item.ReservationKey != exchange.ExchangeKey ) ||
                 ( item.Version != ( line.ExpectedItemVersion + 1 ) ) )
            {
                throw new InventoryException( "Exchange reservation is no longer valid." );
            }

            resolvedLines.Add( ( line, item ) );
        }

        DateTimeOffset cancelledAtUtc = _timeProvider.GetUtcNow();
        exchange.Cancel( request.OperationId, cancelledAtUtc );
        foreach ( ( PartyExchangeLine line, ItemInstance item ) in resolvedLines )
        {
            item.ReleaseReservation(
                exchange.ExchangeKey,
                line.ExpectedItemVersion + 1,
                request.OperationId,
                cancelledAtUtc );
        }

        _repository.AddAudit( InventoryAuditFactory.CreatePlayerAction(
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.ExchangeCancelled,
            request.ActingUserId,
            "Party exchange cancelled and reservations released.",
            null,
            exchange.ExchangeKey,
            cancelledAtUtc ) );

        await _repository.SaveChangesAsync( cancellationToken );
        return exchange.ToDto();
    }
}
