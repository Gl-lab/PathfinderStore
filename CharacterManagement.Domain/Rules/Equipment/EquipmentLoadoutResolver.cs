using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Rules.Equipment;

public sealed record EquipmentProficiencyMatch(
    string EquipmentId,
    string? ProficiencyTargetId,
    ProficiencyRank Rank );

public sealed record EquipmentLoadoutResult(
    IReadOnlyList<CharacterEquipmentItem> Items,
    IReadOnlyList<EquipmentProficiencyMatch> Proficiencies,
    int TotalBulkTenths,
    int EncumberedAtBulkTenths,
    int MaximumBulkTenths )
{
    public bool IsEncumbered => TotalBulkTenths > EncumberedAtBulkTenths;
    public bool ExceedsMaximumBulk => TotalBulkTenths > MaximumBulkTenths;
}

public static class EquipmentLoadoutResolver
{
    public static EquipmentLoadoutResult Resolve(
        IReadOnlyList<CharacterEquipmentItem> items,
        IReadOnlyCollection<EquipmentDefinition> catalog,
        IReadOnlyList<string> equippedEquipmentIds,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        int strengthModifier )
    {
        ArgumentNullException.ThrowIfNull( items );
        ArgumentNullException.ThrowIfNull( catalog );
        ArgumentNullException.ThrowIfNull( equippedEquipmentIds );
        ArgumentNullException.ThrowIfNull( proficiencies );

        if ( equippedEquipmentIds.Any( String.IsNullOrWhiteSpace ) ||
             equippedEquipmentIds.Distinct( StringComparer.Ordinal ).Count() != equippedEquipmentIds.Count )
        {
            throw new CharacterManagementException( "Equipped equipment ids must be non-empty and unique." );
        }

        Dictionary<string, EquipmentDefinition> definitions = catalog
            .ToDictionary( definition => definition.Id, StringComparer.Ordinal );
        HashSet<string> equippedIds = equippedEquipmentIds.ToHashSet( StringComparer.Ordinal );
        List<CharacterEquipmentItem> resolvedItems = [];
        List<EquipmentProficiencyMatch> matches = [];
        int equippedArmorCount = 0;
        int totalBulkTenths = 0;

        foreach ( CharacterEquipmentItem item in items )
        {
            if ( !definitions.TryGetValue( item.EquipmentId, out EquipmentDefinition? definition ) )
            {
                throw new CharacterManagementException( $"Equipment '{item.EquipmentId}' is absent from the catalog." );
            }

            bool isEquipped = equippedIds.Remove( item.EquipmentId );
            if ( isEquipped &&
                 definition.Category != EquipmentCategory.Weapon &&
                 definition.Category != EquipmentCategory.Armor &&
                 definition.Category != EquipmentCategory.Shield )
            {
                throw new CharacterManagementException( $"Equipment '{definition.Id}' cannot be equipped." );
            }

            if ( isEquipped && definition.Category == EquipmentCategory.Armor )
            {
                equippedArmorCount++;
            }

            resolvedItems.Add( item with { EquippedQuantity = isEquipped ? 1 : 0 } );
            totalBulkTenths += definition.BulkTenths * item.PurchaseQuantity;
            matches.Add( MatchProficiency( definition, proficiencies ) );
        }

        if ( equippedIds.Count > 0 )
        {
            throw new CharacterManagementException(
                $"Equipped equipment '{equippedIds.First()}' is not present in the character inventory." );
        }

        if ( equippedArmorCount > 1 )
        {
            throw new CharacterManagementException( "Only one armor item can be equipped." );
        }

        int encumberedAtBulkTenths = Math.Max( 0, 5 + strengthModifier ) * 10;
        int maximumBulkTenths = Math.Max( 0, 10 + strengthModifier ) * 10;
        return new EquipmentLoadoutResult(
            resolvedItems,
            matches,
            totalBulkTenths,
            encumberedAtBulkTenths,
            maximumBulkTenths );
    }

    private static EquipmentProficiencyMatch MatchProficiency(
        EquipmentDefinition definition,
        IReadOnlyList<EffectiveProficiency> proficiencies )
    {
        if ( definition.Category == EquipmentCategory.Weapon )
        {
            return MatchWeaponProficiency( definition, proficiencies );
        }

        string? targetId = definition.Category switch
        {
            EquipmentCategory.Armor => definition.Armor?.Category switch
            {
                EquipmentArmorCategory.Unarmored => ProficiencyTargets.UnarmoredDefense.Id,
                EquipmentArmorCategory.Light => ProficiencyTargets.LightArmor.Id,
                EquipmentArmorCategory.Medium => ProficiencyTargets.MediumArmor.Id,
                _ => null,
            },
            _ => null,
        };
        EffectiveProficiency? proficiency = targetId is null
            ? null
            : proficiencies.FirstOrDefault( item => item.Target.Id == targetId );
        return new EquipmentProficiencyMatch(
            definition.Id,
            targetId,
            proficiency?.Rank ?? ProficiencyRank.Untrained );
    }

    private static EquipmentProficiencyMatch MatchWeaponProficiency(
        EquipmentDefinition definition,
        IReadOnlyList<EffectiveProficiency> proficiencies )
    {
        string specificTargetId = $"proficiency.attack.weapon.{definition.Id[ "equipment.".Length.. ]}";
        string? categoryTargetId = definition.Weapon?.Category switch
        {
            EquipmentWeaponCategory.Simple => ProficiencyTargets.SimpleWeapons.Id,
            EquipmentWeaponCategory.Martial => ProficiencyTargets.MartialWeapons.Id,
            _ => null,
        };
        EffectiveProficiency? best = proficiencies
            .Where( item => item.Target.Id == specificTargetId ||
                            item.Target.Id == categoryTargetId )
            .OrderByDescending( item => item.Rank )
            .FirstOrDefault();
        return new EquipmentProficiencyMatch(
            definition.Id,
            best?.Target.Id ?? categoryTargetId,
            best?.Rank ?? ProficiencyRank.Untrained );
    }
}
