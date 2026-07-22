using MediatR;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed record CreatePartyExchangeCommand(
    int ActingUserId,
    int CampaignId,
    Guid ExchangeKey,
    int InitiatorCharacterId,
    int CounterpartyCharacterId,
    IReadOnlyCollection<CreatePartyExchangeLine> Lines ) : IRequest<PartyExchangeDto>;

public sealed record CreatePartyExchangeLine(
    int FromCharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion,
    Guid ReservationOperationId );
