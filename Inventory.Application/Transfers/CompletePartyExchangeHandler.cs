using MediatR;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Application.Audit;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed class CompletePartyExchangeHandler
    : IRequestHandler<CompletePartyExchangeCommand, PartyExchangeDto>
{
    private readonly IInventoryTransferRepository _repository;
    private readonly IPartyTransferAccessPolicy _accessPolicy;
    private readonly IItemTransferRestrictionPolicy _restrictionPolicy;
    private readonly TimeProvider _timeProvider;

    public CompletePartyExchangeHandler(
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
        CompletePartyExchangeCommand request,
        CancellationToken cancellationToken )
    {
        PartyExchange exchange = await _repository.GetExchangeAsync(
            request.ExchangeKey,
            cancellationToken ) ?? throw new InventoryException( "Exchange was not found." );
        EnsureCampaign( exchange, request.CampaignId );
        PartyTransferAccess access = await GetAccessAsync(
            exchange,
            request.ActingUserId,
            cancellationToken );
        if ( !access.ControlsDestination )
        {
            throw new InventoryException( "Only the counterparty can confirm the exchange." );
        }

        if ( exchange.Status == PartyExchangeStatus.Completed )
        {
            exchange.Complete(
                request.OperationId,
                exchange.CompletedAtUtc!.Value );
            return exchange.ToDto();
        }

        InventoryContainer initiatorContainer = await GetContainerAsync(
            exchange.CampaignId,
            exchange.InitiatorCharacterId,
            cancellationToken );
        InventoryContainer counterpartyContainer = await GetContainerAsync(
            exchange.CampaignId,
            exchange.CounterpartyCharacterId,
            cancellationToken );
        List<( PartyExchangeLine Line, ItemInstance Item )> resolvedLines = [];
        foreach ( PartyExchangeLine line in exchange.Lines )
        {
            InventoryContainer source = line.FromCharacterId == exchange.InitiatorCharacterId
                ? initiatorContainer
                : counterpartyContainer;
            ItemInstance item = await _repository.GetItemAsync(
                line.ItemInstanceKey,
                cancellationToken ) ?? throw new InventoryException( "Exchange item was not found." );
            if ( ( item.CurrentContainerKey != source.ContainerKey ) ||
                 item.IsDepleted ||
                 ( item.ReservationKey != exchange.ExchangeKey ) ||
                 ( item.Version != ( line.ExpectedItemVersion + 1 ) ) )
            {
                throw new InventoryException( "Exchange item changed after reservation." );
            }

            bool isEquipped = await _restrictionPolicy.IsEquippedAsync(
                line.FromCharacterId,
                line.ItemInstanceKey,
                cancellationToken );
            if ( isEquipped )
            {
                throw new InventoryException( "An equipped item cannot be exchanged." );
            }

            resolvedLines.Add( ( line, item ) );
        }

        DateTimeOffset completedAtUtc = _timeProvider.GetUtcNow();
        exchange.Complete( request.OperationId, completedAtUtc );
        foreach ( ( PartyExchangeLine line, ItemInstance item ) in resolvedLines )
        {
            InventoryContainer destination = line.FromCharacterId == exchange.InitiatorCharacterId
                ? counterpartyContainer
                : initiatorContainer;
            item.MoveReservedTo(
                destination,
                exchange.ExchangeKey,
                line.ExpectedItemVersion + 1,
                request.OperationId,
                $"user:{request.ActingUserId}",
                completedAtUtc );
        }

        _repository.AddAudit( InventoryAuditFactory.CreatePlayerAction(
            request.CampaignId,
            request.OperationId,
            InventoryAuditActionKind.ExchangeCompleted,
            request.ActingUserId,
            "Party exchange completed atomically.",
            null,
            exchange.ExchangeKey,
            completedAtUtc ) );

        await _repository.SaveChangesAsync( cancellationToken );
        return exchange.ToDto();
    }

    private async Task<PartyTransferAccess> GetAccessAsync(
        PartyExchange exchange,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        PartyTransferAccess access = await _accessPolicy.GetAccessAsync(
            exchange.CampaignId,
            actingUserId,
            exchange.InitiatorCharacterId,
            exchange.CounterpartyCharacterId,
            cancellationToken );
        if ( !access.SameActiveParty || ( access.PartyId != exchange.PartyId ) )
        {
            throw new InventoryException( "Exchange participants are no longer in the same active party." );
        }

        return access;
    }

    private async Task<InventoryContainer> GetContainerAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken )
    {
        return await _repository.GetCharacterContainerAsync(
            campaignId,
            characterId,
            cancellationToken ) ?? throw new InventoryException( "Exchange character inventory was not found." );
    }

    private static void EnsureCampaign( PartyExchange exchange, int campaignId )
    {
        if ( exchange.CampaignId != campaignId )
        {
            throw new InventoryException( "Exchange does not belong to this campaign." );
        }
    }
}
