namespace Pathfinder.Inventory.Domain.Operations;

public enum InventoryOperationKind
{
    Split = 0,
    Merge = 1,
    Move = 2,
    Reserve = 3,
    ReleaseReservation = 4,
    RestrictTransfer = 5,
    AllowTransfer = 6,
}
