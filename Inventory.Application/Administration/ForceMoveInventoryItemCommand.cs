using MediatR;

namespace Pathfinder.Inventory.Application.Administration;

public sealed record ForceMoveInventoryItemCommand(
    int ActingUserId,
    int CampaignId,
    Guid ItemInstanceKey,
    Guid DestinationContainerKey,
    int ExpectedItemVersion,
    Guid OperationId,
    string Reason ) : IRequest<ForcedInventoryMoveDto>;

public sealed record ForcedInventoryMoveDto(
    Guid ItemInstanceKey,
    Guid DestinationContainerKey,
    int Version,
    Guid AuditKey );
