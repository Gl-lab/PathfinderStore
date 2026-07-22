using MediatR;

namespace Pathfinder.Inventory.Application.Storage;

public sealed record WithdrawPartyStorageCommand(
    int ActingUserId,
    int CampaignId,
    int CharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion,
    Guid OperationId ) : IRequest<PartyStorageItemDto>;
