using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Equipment;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using CharacterArmorCategory = Pathfinder.CharacterManagement.Domain.Entity.EquipmentArmorCategory;
using CharacterEquipmentCategory = Pathfinder.CharacterManagement.Domain.Entity.EquipmentCategory;

namespace Pathfinder.Web.Integration;

public sealed class ItemCatalogAllowedEquipmentReader : IAllowedEquipmentReader
{
    private readonly IEquipmentRepository _startingEquipmentRepository;
    private readonly ItemCatalogDbContext _itemCatalogDbContext;

    public ItemCatalogAllowedEquipmentReader(
        IEquipmentRepository startingEquipmentRepository,
        ItemCatalogDbContext itemCatalogDbContext )
    {
        _startingEquipmentRepository = startingEquipmentRepository;
        _itemCatalogDbContext = itemCatalogDbContext;
    }

    public AllowedEquipmentLoadout Read(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        int? campaignId = null )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( proficiencies );
        if ( campaignId is <= 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( campaignId ) );
        }

        IReadOnlyCollection<EquipmentDefinition> catalog = BuildVisibleCatalog(
            character.StartingEquipmentItems
                .Select( item => item.EquipmentId )
                .ToHashSet( StringComparer.Ordinal ),
            campaignId );
        IEquipmentRepository overlayRepository = new OverlayEquipmentRepository(
            _startingEquipmentRepository,
            catalog );
        StartingEquipmentAllowedEquipmentReader reader =
            new StartingEquipmentAllowedEquipmentReader( overlayRepository );
        return reader.Read( character, proficiencies, campaignId );
    }

    private IReadOnlyCollection<EquipmentDefinition> BuildVisibleCatalog(
        IReadOnlySet<string> requestedKeys,
        int? campaignId )
    {
        IReadOnlyCollection<EquipmentDefinition> startingCatalog =
            _startingEquipmentRepository.GetAll();
        if ( requestedKeys.Count == 0 )
        {
            return startingCatalog;
        }

        string[] requestedKeyValues = requestedKeys.ToArray();
        ItemDefinition[] visibleDefinitions = _itemCatalogDbContext.ItemDefinitions
            .AsNoTracking()
            .AsSplitQuery()
            .Where( definition =>
                requestedKeyValues.Contains( definition.Key ) &&
                ( ( definition.Scope == ItemCatalogScope.Global ) ||
                  ( campaignId.HasValue &&
                    ( definition.Scope == ItemCatalogScope.Campaign ) &&
                    ( definition.CampaignId == campaignId.Value ) ) ) )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Attacks )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Armor )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Shield )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Durability )
            .ToArray();
        Dictionary<string, ItemRevision> revisions = visibleDefinitions
            .Select( definition => new
            {
                Definition = definition,
                Revision = definition.Revisions.SingleOrDefault(
                    revision => revision.Status == ItemRevisionStatus.Published ),
            } )
            .Where( item => item.Revision is not null )
            .GroupBy( item => item.Definition.Key, StringComparer.Ordinal )
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderByDescending( item => item.Definition.Scope )
                    .Select( item => item.Revision! )
                    .First(),
                StringComparer.Ordinal );

        return startingCatalog
            .Select( definition => revisions.TryGetValue( definition.Id, out ItemRevision? revision )
                ? Overlay( definition, revision )
                : definition )
            .ToArray();
    }

    private static EquipmentDefinition Overlay(
        EquipmentDefinition fallback,
        ItemRevision revision )
    {
        CharacterEquipmentCategory category = MapCategory( revision.PrimaryCategory );
        if ( category != fallback.Category )
        {
            throw new InvalidOperationException(
                $"Published item '{fallback.Id}' changes its starting-equipment category." );
        }

        return new EquipmentDefinition(
            fallback.Id,
            revision.Name,
            category,
            fallback.Rarity,
            revision.PriceInCopperPieces,
            ToBulkTenths( revision.Bulk ),
            fallback.UnitsPerPurchase,
            fallback.Source,
            MapWeapon( fallback, revision ),
            MapArmor( fallback, revision ),
            MapShield( fallback, revision ),
            fallback.Ammunition );
    }

    private static CharacterEquipmentCategory MapCategory( ItemCategory category ) => category switch
    {
        ItemCategory.Weapon => CharacterEquipmentCategory.Weapon,
        ItemCategory.Armor => CharacterEquipmentCategory.Armor,
        ItemCategory.Shield => CharacterEquipmentCategory.Shield,
        ItemCategory.Ammunition => CharacterEquipmentCategory.Ammunition,
        ItemCategory.Consumable or ItemCategory.Tool or ItemCategory.Container or
            ItemCategory.OtherEquipment => CharacterEquipmentCategory.Gear,
        _ => throw new InvalidOperationException(
            $"Item category '{category}' cannot be used as starting equipment." ),
    };

    private static EquipmentWeaponStatistics? MapWeapon(
        EquipmentDefinition fallback,
        ItemRevision revision )
    {
        if ( fallback.Category != CharacterEquipmentCategory.Weapon )
        {
            return null;
        }

        EquipmentWeaponStatistics fallbackWeapon = fallback.Weapon
            ?? throw new InvalidOperationException( $"Weapon '{fallback.Id}' has no fallback rules." );
        AttackComponent attack = revision.Attacks.SingleOrDefault()
            ?? throw new InvalidOperationException(
                $"Published weapon '{fallback.Id}' must have exactly one attack component." );
        if ( attack.DamageDieCount != 1 )
        {
            throw new InvalidOperationException(
                $"Published weapon '{fallback.Id}' is incompatible with the combat-card v1 damage model." );
        }

        return new EquipmentWeaponStatistics(
            fallbackWeapon.Category,
            fallbackWeapon.Group,
            attack.RangeIncrementFeet.HasValue
                ? EquipmentWeaponType.Ranged
                : EquipmentWeaponType.Melee,
            ( int )attack.DamageDieSize,
            attack.DamageType.ToString(),
            attack.Hands.ToString( CultureInfo.InvariantCulture ),
            attack.RangeIncrementFeet,
            fallbackWeapon.Traits.ToArray() );
    }

    private static EquipmentArmorStatistics? MapArmor(
        EquipmentDefinition fallback,
        ItemRevision revision )
    {
        if ( fallback.Category != CharacterEquipmentCategory.Armor )
        {
            return null;
        }

        EquipmentArmorStatistics fallbackArmor = fallback.Armor
            ?? throw new InvalidOperationException( $"Armor '{fallback.Id}' has no fallback rules." );
        ArmorComponent armor = revision.Armor
            ?? throw new InvalidOperationException(
                $"Published armor '{fallback.Id}' has no armor component." );
        CharacterArmorCategory category = armor.Category switch
        {
            ArmorCategory.Unarmored => CharacterArmorCategory.Unarmored,
            ArmorCategory.Light => CharacterArmorCategory.Light,
            ArmorCategory.Medium => CharacterArmorCategory.Medium,
            _ => throw new InvalidOperationException(
                $"Published armor '{fallback.Id}' is not supported by combat-card v1." ),
        };
        return new EquipmentArmorStatistics(
            category,
            fallbackArmor.Group,
            armor.ArmorClassBonus,
            armor.DexterityCap,
            armor.CheckPenalty,
            armor.SpeedPenaltyFeet,
            armor.StrengthRequirement,
            fallbackArmor.Traits.ToArray() );
    }

    private static EquipmentShieldStatistics? MapShield(
        EquipmentDefinition fallback,
        ItemRevision revision )
    {
        if ( fallback.Category != CharacterEquipmentCategory.Shield )
        {
            return null;
        }

        ShieldComponent shield = revision.Shield
            ?? throw new InvalidOperationException(
                $"Published shield '{fallback.Id}' has no shield component." );
        DurabilityComponent durability = revision.Durability
            ?? throw new InvalidOperationException(
                $"Published shield '{fallback.Id}' has no durability component." );
        return new EquipmentShieldStatistics(
            shield.RaisedArmorClassBonus,
            durability.Hardness,
            durability.MaximumHitPoints );
    }

    private static int ToBulkTenths( decimal bulk )
    {
        decimal tenths = bulk * 10;
        if ( tenths != Decimal.Truncate( tenths ) )
        {
            throw new InvalidOperationException(
                "Published item Bulk must use tenths for combat-card v1." );
        }

        return Decimal.ToInt32( tenths );
    }

    private sealed class OverlayEquipmentRepository : IEquipmentRepository
    {
        private readonly IEquipmentRepository _fallback;
        private readonly IReadOnlyDictionary<string, EquipmentDefinition> _equipment;

        public OverlayEquipmentRepository(
            IEquipmentRepository fallback,
            IReadOnlyCollection<EquipmentDefinition> equipment )
        {
            _fallback = fallback;
            _equipment = equipment.ToDictionary( item => item.Id, StringComparer.Ordinal );
        }

        public IReadOnlyCollection<EquipmentDefinition> GetAll() => _equipment.Values.ToArray();

        public EquipmentDefinition GetEquipment( string equipmentId ) =>
            _equipment.TryGetValue( equipmentId, out EquipmentDefinition? equipment )
                ? equipment
                : throw new ArgumentOutOfRangeException( nameof( equipmentId ) );

        public IReadOnlyCollection<ClassKitDefinition> GetClassKits() => _fallback.GetClassKits();

        public ClassKitDefinition GetClassKit( string characterClassId ) =>
            _fallback.GetClassKit( characterClassId );
    }
}
