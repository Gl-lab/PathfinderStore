using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class EquipmentDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public EquipmentCategory Category { get; set; }
    public EquipmentRarity Rarity { get; set; }
    public int PriceCopper { get; set; }
    public int BulkTenths { get; set; }
    public int UnitsPerPurchase { get; set; }
    public SourceReference Source { get; set; } = SourceReference.Unknown;
    public EquipmentWeaponStatistics? Weapon { get; set; }
    public EquipmentArmorStatistics? Armor { get; set; }
    public EquipmentShieldStatistics? Shield { get; set; }
    public EquipmentAmmunitionStatistics? Ammunition { get; set; }
}

public sealed class ClassKitDto
{
    public string Id { get; set; } = String.Empty;
    public string CharacterClassId { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public int StartingWealthCopper { get; set; }
    public IReadOnlyList<ClassKitItem> Items { get; set; } = [];
    public IReadOnlyList<ClassKitOptionGroup> OptionGroups { get; set; } = [];
    public SourceReference Source { get; set; } = SourceReference.Unknown;
}
