namespace Pathfinder.Commerce.Domain.Restocking;

[Flags]
public enum RestockItemRarity
{
    Common = 1,
    Uncommon = 2,
    Rare = 4,
    Unique = 8,
    All = Common | Uncommon | Rare | Unique,
}
