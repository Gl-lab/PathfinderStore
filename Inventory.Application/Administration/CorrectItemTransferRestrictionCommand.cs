using MediatR;

namespace Pathfinder.Inventory.Application.Administration;

public sealed record CorrectItemTransferRestrictionCommand(
    int ActingUserId,
    int CampaignId,
    Guid ItemInstanceKey,
    bool IsTransferRestricted,
    int ExpectedItemVersion,
    Guid OperationId,
    string Reason ) : IRequest<CorrectedItemTransferRestrictionDto>;

public sealed record CorrectedItemTransferRestrictionDto(
    Guid ItemInstanceKey,
    bool IsTransferRestricted,
    int Version,
    Guid AuditKey );