using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed record PartyExchangeDto(
    Guid ExchangeKey,
    int CampaignId,
    int PartyId,
    int InitiatorCharacterId,
    int CounterpartyCharacterId,
    PartyExchangeStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    int Version,
    DateTimeOffset? CompletedAtUtc,
    DateTimeOffset? CancelledAtUtc,
    Guid? FinalOperationId,
    IReadOnlyCollection<PartyExchangeLineDto> Lines );

public sealed record PartyExchangeLineDto(
    int FromCharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion );

internal static class PartyExchangeMappings
{
    internal static PartyExchangeDto ToDto( this PartyExchange exchange ) => new(
        exchange.ExchangeKey,
        exchange.CampaignId,
        exchange.PartyId,
        exchange.InitiatorCharacterId,
        exchange.CounterpartyCharacterId,
        exchange.Status,
        exchange.CreatedAtUtc,
        exchange.ExpiresAtUtc,
        exchange.Version,
        exchange.CompletedAtUtc,
        exchange.CancelledAtUtc,
        exchange.FinalOperationId,
        exchange.Lines
            .Select( line => new PartyExchangeLineDto(
                line.FromCharacterId,
                line.ItemInstanceKey,
                line.ExpectedItemVersion ) )
            .ToArray() );
}
