using MediatR;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed record CreatePartyGiftCommand(
    int ActingUserId,
    int CampaignId,
    Guid GiftKey,
    int SourceCharacterId,
    int DestinationCharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion ) : IRequest<PartyGiftDto>;
