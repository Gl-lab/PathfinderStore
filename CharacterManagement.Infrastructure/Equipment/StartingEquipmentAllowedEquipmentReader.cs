using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;

namespace Pathfinder.CharacterManagement.Infrastructure.Equipment;

public sealed class StartingEquipmentAllowedEquipmentReader : IAllowedEquipmentReader
{
    private readonly IEquipmentRepository _equipmentRepository;

    public StartingEquipmentAllowedEquipmentReader( IEquipmentRepository equipmentRepository )
    {
        _equipmentRepository = equipmentRepository;
    }

    public AllowedEquipmentLoadout Read(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( proficiencies );

        EquipmentLoadoutResult loadout = EquipmentLoadoutResolver.Resolve(
            character.StartingEquipmentItems,
            _equipmentRepository.GetAll(),
            character.StartingEquipmentItems
                .Where( item => item.EquippedQuantity > 0 )
                .Select( item => item.EquipmentId )
                .ToArray(),
            proficiencies,
            character.AbilityScores.Strength.Modifier );
        IReadOnlyDictionary<string, EquipmentProficiencyMatch> proficiencyMatches = loadout.Proficiencies
            .ToDictionary( match => match.EquipmentId, StringComparer.Ordinal );
        IReadOnlyList<AllowedEquipmentItem> items = loadout.Items
            .Select( item => MapItem( item, proficiencyMatches[ item.EquipmentId ] ) )
            .ToArray();
        int totalPriceCopper = items.Sum( item => item.PriceCopper * item.PurchaseQuantity );

        return new AllowedEquipmentLoadout(
            items,
            totalPriceCopper,
            ClassKitDefinition.StartingWealthCopper - totalPriceCopper,
            loadout.TotalBulkTenths,
            loadout.EncumberedAtBulkTenths,
            loadout.MaximumBulkTenths );
    }

    private AllowedEquipmentItem MapItem(
        CharacterEquipmentItem item,
        EquipmentProficiencyMatch proficiencyMatch )
    {
        EquipmentDefinition definition = _equipmentRepository.GetEquipment( item.EquipmentId );
        return new AllowedEquipmentItem(
            definition.Id,
            definition.Name,
            definition.Category,
            definition.PriceCopper,
            definition.BulkTenths,
            definition.UnitsPerPurchase,
            item.PurchaseQuantity,
            item.EquippedQuantity,
            proficiencyMatch.ProficiencyTargetId,
            proficiencyMatch.Rank,
            MapWeapon( definition.Weapon ),
            MapArmor( definition.Armor ),
            MapShield( definition.Shield ) );
    }

    private static AllowedWeaponStatistics? MapWeapon( EquipmentWeaponStatistics? weapon )
    {
        return weapon is null
            ? null
            : new AllowedWeaponStatistics(
                weapon.Category,
                weapon.Group,
                weapon.Type,
                weapon.DamageDie,
                weapon.DamageType,
                weapon.Hands,
                weapon.RangeFeet,
                weapon.Traits.ToArray() );
    }

    private static AllowedArmorStatistics? MapArmor( EquipmentArmorStatistics? armor )
    {
        return armor is null
            ? null
            : new AllowedArmorStatistics(
                armor.Category,
                armor.Group,
                armor.ArmorClassBonus,
                armor.DexterityCap,
                armor.CheckPenalty,
                armor.SpeedPenaltyFeet,
                armor.StrengthThreshold,
                armor.Traits.ToArray() );
    }

    private static AllowedShieldStatistics? MapShield( EquipmentShieldStatistics? shield )
    {
        return shield is null
            ? null
            : new AllowedShieldStatistics(
                shield.ArmorClassBonus,
                shield.Hardness,
                shield.HitPoints );
    }
}
