using MediatR;

namespace Pathfinder.Inventory.Application.Storage;

public sealed record DepositPartyStorageCommand(
    int ActingUserId,
    int CampaignId,
    int CharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion,
    Guid OperationId ) : IRequest<PartyStorageItemDto>;
