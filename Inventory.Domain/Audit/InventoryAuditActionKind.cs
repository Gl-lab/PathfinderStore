namespace Pathfinder.Inventory.Domain.Audit;

public enum InventoryAuditActionKind
{
    GiftProposed = 0,
    GiftAccepted = 1,
    ExchangeProposed = 2,
    ExchangeCompleted = 3,
    ExchangeCancelled = 4,
    PartyStorageDeposited = 5,
    PartyStorageWithdrawn = 6,
    ForcedMove = 7,
}
