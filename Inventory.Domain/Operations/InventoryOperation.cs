using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Operations;

public sealed class InventoryOperation : Entity
{
    private InventoryOperation()
    {
    }

    public Guid OperationId { get; private set; }
    public InventoryOperationKind Kind { get; private set; }
    public Guid RelatedKey { get; private set; }
    public int Quantity { get; private set; }
    public int VersionAfter { get; private set; }
    public DateTimeOffset AppliedAtUtc { get; private set; }

    internal static InventoryOperation Create(
        Guid operationId,
        InventoryOperationKind kind,
        Guid relatedKey,
        int quantity,
        int versionAfter,
        DateTimeOffset appliedAtUtc )
    {
        return new InventoryOperation
        {
            OperationId = operationId,
            Kind = kind,
            RelatedKey = relatedKey,
            Quantity = quantity,
            VersionAfter = versionAfter,
            AppliedAtUtc = appliedAtUtc,
        };
    }

    internal void EnsureMatches(
        InventoryOperationKind kind,
        Guid relatedKey,
        int quantity )
    {
        if ( ( Kind != kind ) ||
             ( RelatedKey != relatedKey ) ||
             ( Quantity != quantity ) )
        {
            throw new InventoryException(
                "Operation id was already used for a different inventory change." );
        }
    }
}
