using MediatR;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed record AcceptPartyGiftCommand(
    int ActingUserId,
    int CampaignId,
    Guid GiftKey,
    Guid OperationId ) : IRequest<PartyGiftDto>;
