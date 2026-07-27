namespace Pathfinder.Commerce.Domain.Restocking;

[Flags]
public enum RestockItemCategory
{
    Weapon = 1,
    Armor = 2,
    Shield = 4,
    Consumable = 8,
    Ammunition = 16,
    Rune = 32,
    Tool = 64,
    Container = 128,
    OtherEquipment = 256,
    All = Weapon | Armor | Shield | Consumable | Ammunition | Rune | Tool | Container |
          OtherEquipment,
}
