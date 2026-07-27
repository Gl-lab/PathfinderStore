namespace Pathfinder.Commerce.Domain.Restocking;

public sealed record RestockCandidate(
    int ItemConfigurationId,
    int Level,
    long UnitPriceCopper,
    RestockItemRarity Rarity,
    RestockItemAccess Access,
    RestockItemCategory Category,
    RestockItemKind Kind );
