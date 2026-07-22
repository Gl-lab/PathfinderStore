using MediatR;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed record CompletePartyExchangeCommand(
    int ActingUserId,
    int CampaignId,
    Guid ExchangeKey,
    Guid OperationId ) : IRequest<PartyExchangeDto>;
