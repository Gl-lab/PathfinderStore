using Pathfinder.Inventory.Domain.Audit;

namespace Pathfinder.Inventory.Application.Audit;

public static class InventoryAuditFactory
{
    public static InventoryAuditEntry CreatePlayerAction(
        int campaignId,
        Guid operationId,
        InventoryAuditActionKind actionKind,
        int actorUserId,
        string reason,
        Guid? itemInstanceKey,
        Guid? relatedKey,
        DateTimeOffset occurredAtUtc )
    {
        return InventoryAuditEntry.Create(
            Guid.NewGuid(),
            campaignId,
            operationId,
            actionKind,
            actorUserId,
            false,
            reason,
            itemInstanceKey,
            relatedKey,
            occurredAtUtc );
    }
}
