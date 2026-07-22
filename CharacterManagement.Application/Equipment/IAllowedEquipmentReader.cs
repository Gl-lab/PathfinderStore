using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Equipment;

public interface IAllowedEquipmentReader
{
    AllowedEquipmentLoadout Read(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        int? campaignId = null );
}

public sealed record AllowedEquipmentLoadout(
    IReadOnlyList<AllowedEquipmentItem> Items,
    int TotalPriceCopper,
    int RemainingWealthCopper,
    int TotalBulkTenths,
    int EncumberedAtBulkTenths,
    int MaximumBulkTenths )
{
    public bool IsEncumbered => TotalBulkTenths > EncumberedAtBulkTenths;
    public bool ExceedsMaximumBulk => TotalBulkTenths > MaximumBulkTenths;
}

public sealed record AllowedEquipmentItem(
    string Id,
    string Name,
    EquipmentCategory Category,
    int PriceCopper,
    int BulkTenths,
    int UnitsPerPurchase,
    int PurchaseQuantity,
    int EquippedQuantity,
    string? ProficiencyTargetId,
    ProficiencyRank ProficiencyRank,
    AllowedWeaponStatistics? Weapon,
    AllowedArmorStatistics? Armor,
    AllowedShieldStatistics? Shield )
{
    public int UnitQuantity => PurchaseQuantity * UnitsPerPurchase;
}

public sealed record AllowedWeaponStatistics(
    EquipmentWeaponCategory Category,
    string Group,
    EquipmentWeaponType Type,
    int DamageDie,
    string DamageType,
    string Hands,
    int? RangeFeet,
    IReadOnlyList<string> Traits );

public sealed record AllowedArmorStatistics(
    EquipmentArmorCategory Category,
    string Group,
    int ArmorClassBonus,
    int DexterityCap,
    int CheckPenalty,
    int SpeedPenaltyFeet,
    int StrengthThreshold,
    IReadOnlyList<string> Traits );

public sealed record AllowedShieldStatistics(
    int ArmorClassBonus,
    int Hardness,
    int HitPoints );
