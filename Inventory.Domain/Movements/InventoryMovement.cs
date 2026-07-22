using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Movements;

public sealed class InventoryMovement : Entity
{
    private InventoryMovement()
    {
    }

    public Guid ItemInstanceKey { get; private set; }
    public Guid FromContainerKey { get; private set; }
    public Guid ToContainerKey { get; private set; }
    public int Quantity { get; private set; }
    public string Reason { get; private set; } = String.Empty;
    public Guid OperationId { get; private set; }
    public string PerformedBy { get; private set; } = String.Empty;
    public DateTimeOffset OccurredAtUtc { get; private set; }

    internal static InventoryMovement Create(
        Guid itemInstanceKey,
        Guid fromContainerKey,
        Guid toContainerKey,
        int quantity,
        string reason,
        Guid operationId,
        string performedBy,
        DateTimeOffset occurredAtUtc )
    {
        return new InventoryMovement
        {
            ItemInstanceKey = itemInstanceKey,
            FromContainerKey = fromContainerKey,
            ToContainerKey = toContainerKey,
            Quantity = quantity,
            Reason = reason,
            OperationId = operationId,
            PerformedBy = performedBy,
            OccurredAtUtc = occurredAtUtc,
        };
    }
}
