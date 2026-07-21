using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class EquipmentRepository : IEquipmentRepository
{
    private static readonly Dictionary<string, EquipmentDefinition> _equipment = CreateEquipment()
        .ToDictionary( definition => definition.Id, StringComparer.Ordinal );
    private static readonly Dictionary<string, ClassKitDefinition> _classKits = CreateClassKits()
        .ToDictionary( definition => definition.CharacterClassId, StringComparer.Ordinal );

    public IReadOnlyCollection<EquipmentDefinition> GetAll() => _equipment.Values.ToArray();

    public EquipmentDefinition GetEquipment( string equipmentId )
    {
        if ( String.IsNullOrWhiteSpace( equipmentId ) )
        {
            throw new ArgumentException( "Equipment id cannot be empty.", nameof( equipmentId ) );
        }

        if ( !_equipment.TryGetValue( equipmentId, out EquipmentDefinition? equipment ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( equipmentId ),
                $"Equipment '{equipmentId}' is not defined." );
        }

        return equipment;
    }

    public IReadOnlyCollection<ClassKitDefinition> GetClassKits() => _classKits.Values.ToArray();

    public ClassKitDefinition GetClassKit( string characterClassId )
    {
        if ( String.IsNullOrWhiteSpace( characterClassId ) )
        {
            throw new ArgumentException( "Character class id cannot be empty.", nameof( characterClassId ) );
        }

        if ( !_classKits.TryGetValue( characterClassId, out ClassKitDefinition? classKit ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( characterClassId ),
                $"Class kit for '{characterClassId}' is not defined." );
        }

        return classKit;
    }

    private static IReadOnlyCollection<EquipmentDefinition> CreateEquipment()
    {
        return
        [
            Armor( "explorers_clothing", "Explorer's Clothing", 10, 1, 273, EquipmentArmorCategory.Unarmored, "Cloth", 0, 5, 0, 0, 0, [ "Comfort" ] ),
            Armor( "leather_armor", "Leather Armor", 200, 10, 273, EquipmentArmorCategory.Light, "Leather", 1, 4, -1, 0, 0 ),
            Armor( "studded_leather_armor", "Studded Leather Armor", 300, 10, 273, EquipmentArmorCategory.Light, "Leather", 2, 3, -1, 0, 1 ),
            Armor( "hide_armor", "Hide Armor", 200, 20, 273, EquipmentArmorCategory.Medium, "Leather", 3, 2, -2, -5, 2 ),
            Armor( "scale_mail", "Scale Mail", 400, 20, 273, EquipmentArmorCategory.Medium, "Composite", 3, 2, -2, -5, 2 ),
            Armor( "chain_mail", "Chain Mail", 600, 20, 273, EquipmentArmorCategory.Medium, "Chain", 4, 1, -2, -5, 3, [ "Flexible", "Noisy" ] ),

            Weapon( "dagger", "Dagger", 20, 1, 277, EquipmentWeaponCategory.Simple, "Knife", EquipmentWeaponType.Melee, 4, "Piercing", "1", null, [ "Agile", "Finesse", "Thrown 10 ft.", "Versatile Slashing" ] ),
            Weapon( "rapier", "Rapier", 200, 10, 278, EquipmentWeaponCategory.Martial, "Sword", EquipmentWeaponType.Melee, 6, "Piercing", "1", null, [ "Deadly d8", "Disarm", "Finesse" ] ),
            Weapon( "sling", "Sling", 0, 1, 280, EquipmentWeaponCategory.Simple, "Sling", EquipmentWeaponType.Ranged, 6, "Bludgeoning", "1", 50, [ "Propulsive" ] ),
            Weapon( "javelin", "Javelin", 10, 1, 280, EquipmentWeaponCategory.Simple, "Dart", EquipmentWeaponType.Ranged, 6, "Piercing", "1", 30, [ "Thrown" ] ),
            Weapon( "spear", "Spear", 10, 10, 277, EquipmentWeaponCategory.Simple, "Spear", EquipmentWeaponType.Melee, 6, "Piercing", "1", null, [ "Thrown 20 ft." ] ),
            Weapon( "greatsword", "Greatsword", 200, 20, 278, EquipmentWeaponCategory.Martial, "Sword", EquipmentWeaponType.Melee, 12, "Slashing", "2", null, [ "Versatile Piercing" ] ),
            Weapon( "longbow", "Longbow", 600, 20, 280, EquipmentWeaponCategory.Martial, "Bow", EquipmentWeaponType.Ranged, 8, "Piercing", "1+", 100, [ "Deadly d10", "Volley 30 ft." ] ),
            Weapon( "longsword", "Longsword", 100, 10, 278, EquipmentWeaponCategory.Martial, "Sword", EquipmentWeaponType.Melee, 8, "Slashing", "1", null, [ "Versatile Piercing" ] ),
            Weapon( "shortsword", "Shortsword", 90, 1, 278, EquipmentWeaponCategory.Martial, "Sword", EquipmentWeaponType.Melee, 6, "Piercing", "1", null, [ "Agile", "Finesse", "Versatile Slashing" ] ),
            Weapon( "sickle", "Sickle", 20, 1, 277, EquipmentWeaponCategory.Simple, "Knife", EquipmentWeaponType.Melee, 4, "Slashing", "1", null, [ "Agile", "Finesse", "Trip" ] ),
            Weapon( "staff", "Staff", 0, 10, 277, EquipmentWeaponCategory.Simple, "Club", EquipmentWeaponType.Melee, 4, "Bludgeoning", "1", null, [ "Two-Hand d8" ] ),
            Weapon( "crossbow", "Crossbow", 300, 10, 280, EquipmentWeaponCategory.Simple, "Crossbow", EquipmentWeaponType.Ranged, 8, "Piercing", "2", 120 ),
            Weapon( "mace", "Mace", 100, 10, 277, EquipmentWeaponCategory.Simple, "Club", EquipmentWeaponType.Melee, 6, "Bludgeoning", "1", null, [ "Shove" ] ),
            Weapon( "whip", "Whip", 10, 10, 278, EquipmentWeaponCategory.Martial, "Flail", EquipmentWeaponType.Melee, 4, "Slashing", "1", null, [ "Disarm", "Finesse", "Nonlethal", "Reach", "Trip" ] ),
            Weapon( "starknife", "Starknife", 200, 1, 278, EquipmentWeaponCategory.Martial, "Knife", EquipmentWeaponType.Melee, 4, "Piercing", "1", null, [ "Agile", "Deadly d6", "Finesse", "Thrown 20 ft.", "Versatile Slashing" ] ),
            Weapon( "trident", "Trident", 100, 10, 278, EquipmentWeaponCategory.Martial, "Spear", EquipmentWeaponType.Melee, 8, "Piercing", "1", null, [ "Thrown 20 ft." ] ),
            Weapon( "falchion", "Falchion", 300, 20, 278, EquipmentWeaponCategory.Martial, "Sword", EquipmentWeaponType.Melee, 10, "Slashing", "2", null, [ "Forceful", "Sweep" ] ),
            Weapon( "greataxe", "Greataxe", 200, 20, 278, EquipmentWeaponCategory.Martial, "Axe", EquipmentWeaponType.Melee, 12, "Slashing", "2", null, [ "Sweep" ] ),
            Weapon( "scimitar", "Scimitar", 100, 10, 278, EquipmentWeaponCategory.Martial, "Sword", EquipmentWeaponType.Melee, 6, "Slashing", "1", null, [ "Forceful", "Sweep" ] ),
            Weapon( "glaive", "Glaive", 100, 20, 278, EquipmentWeaponCategory.Martial, "Polearm", EquipmentWeaponType.Melee, 8, "Slashing", "2", null, [ "Deadly d8", "Forceful", "Reach" ] ),
            Weapon( "warhammer", "Warhammer", 100, 10, 278, EquipmentWeaponCategory.Martial, "Hammer", EquipmentWeaponType.Melee, 8, "Bludgeoning", "1", null, [ "Shove" ] ),
            Weapon( "scythe", "Scythe", 200, 20, 278, EquipmentWeaponCategory.Martial, "Polearm", EquipmentWeaponType.Melee, 10, "Slashing", "2", null, [ "Deadly d10", "Trip" ] ),
            Weapon( "spiked_chain", "Spiked Chain", 300, 10, 278, EquipmentWeaponCategory.Martial, "Flail", EquipmentWeaponType.Melee, 8, "Slashing", "2", null, [ "Disarm", "Finesse", "Trip" ], EquipmentRarity.Uncommon ),

            Ammunition( "arrows", "Arrows", 10, "Bow" ),
            Ammunition( "bolts", "Bolts", 10, "Crossbow" ),
            Ammunition( "sling_bullets", "Sling Bullets", 1, "Sling" ),

            Gear( "adventurers_pack", "Adventurer's Pack", 150, 10, 287 ),
            Gear( "climbing_kit", "Climbing Kit", 50, 10, 287 ),
            Gear( "grappling_hook", "Grappling Hook", 10, 1, 288 ),
            Gear( "healers_toolkit", "Healer's Toolkit", 500, 10, 288 ),
            Gear( "cookware", "Cookware", 100, 20, 288 ),
            Gear( "musical_instrument_handheld", "Musical Instrument (Handheld)", 80, 10, 290 ),
            Gear( "primal_symbol", "Primal Symbol", 0, 0, 290 ),
            Gear( "religious_symbol_wooden", "Religious Symbol (Wooden)", 10, 1, 290 ),
            Gear( "thieves_toolkit", "Thieves' Toolkit", 300, 1, 292 ),
            Gear( "writing_set", "Writing Set", 100, 1, 292 ),

            new EquipmentDefinition(
                "equipment.steel_shield",
                "Steel Shield",
                EquipmentCategory.Shield,
                EquipmentRarity.Common,
                200,
                10,
                1,
                new SourceReference( "Player Core", 274 ),
                shield: new EquipmentShieldStatistics( 2, 5, 20 ) ),
        ];
    }

    private static IReadOnlyCollection<ClassKitDefinition> CreateClassKits()
    {
        SourceReference source = new SourceReference( "Player Core", 268 );
        return
        [
            Kit( "bard", [ I( "studded_leather_armor" ), I( "dagger" ), I( "rapier" ), I( "sling" ), I( "sling_bullets", 2 ), I( "adventurers_pack" ), I( "musical_instrument_handheld" ) ], [], source ),
            Kit( "cleric", [ I( "explorers_clothing" ), I( "adventurers_pack" ), I( "religious_symbol_wooden" ) ],
                [ Any( "cleric_additions", Dependency( "favored_weapon", "Deity's favored weapon", ClassKitOptionDependency.DeityFavoredWeapon ), Option( "chain_mail", "Chain mail", I( "chain_mail" ) ), Option( "healers_toolkit", "Healer's toolkit", I( "healers_toolkit" ) ) ) ], source ),
            Kit( "druid", [ I( "hide_armor" ), I( "javelin", 4 ), I( "spear" ), I( "adventurers_pack" ), I( "primal_symbol" ) ],
                [ Any( "druid_additions", Option( "healers_toolkit", "Healer's toolkit", I( "healers_toolkit" ) ) ) ], source ),
            Kit( "fighter", [ I( "scale_mail" ), I( "dagger" ), I( "adventurers_pack" ), I( "grappling_hook" ) ],
                [ AtMostOne( "fighter_weapon_package", Option( "greatsword", "Greatsword", I( "greatsword" ) ), Option( "longbow", "Longbow with 20 arrows", I( "longbow" ), I( "arrows", 2 ) ), Option( "sword_and_shield", "Longsword and steel shield", I( "longsword" ), I( "steel_shield" ) ) ) ], source ),
            Kit( "ranger", [ I( "leather_armor" ), I( "dagger" ), I( "adventurers_pack" ) ],
                [ AtMostOne( "ranger_weapon_package", Option( "longbow", "Longbow with 20 arrows", I( "longbow" ), I( "arrows", 2 ) ), Option( "sword_and_shield", "Longsword and steel shield", I( "longsword" ), I( "steel_shield" ) ), Option( "two_shortswords", "Two shortswords", I( "shortsword", 2 ) ) ) ], source ),
            Kit( "rogue", [ I( "leather_armor" ), I( "dagger" ), I( "rapier" ), I( "adventurers_pack" ), I( "climbing_kit" ) ],
                [ Any( "rogue_additions", Option( "thieves_toolkit", "Thieves' toolkit", I( "thieves_toolkit" ) ) ) ], source ),
            Kit( "witch", [ I( "explorers_clothing" ), I( "sickle" ), I( "sling" ), I( "sling_bullets", 2 ), I( "staff" ), I( "adventurers_pack" ) ],
                [ Any( "witch_additions", Option( "cookware", "Cookware", I( "cookware" ) ), Option( "healers_toolkit", "Healer's toolkit", I( "healers_toolkit" ) ) ) ], source ),
            Kit( "wizard", [ I( "explorers_clothing" ), I( "staff" ), I( "adventurers_pack" ), I( "writing_set" ) ],
                [ Any( "wizard_additions", Option( "crossbow", "Crossbow with 20 bolts", I( "crossbow" ), I( "bolts", 2 ) ) ) ], source ),
        ];
    }

    private static EquipmentDefinition Gear( string id, string name, int price, int bulk, int page ) =>
        new EquipmentDefinition( $"equipment.{id}", name, EquipmentCategory.Gear, EquipmentRarity.Common, price, bulk, 1, new SourceReference( "Player Core", page ) );

    private static EquipmentDefinition Ammunition( string id, string name, int price, string group ) =>
        new EquipmentDefinition(
            $"equipment.{id}",
            name,
            EquipmentCategory.Ammunition,
            EquipmentRarity.Common,
            price,
            1,
            10,
            new SourceReference( "Player Core", 280 ),
            ammunition: new EquipmentAmmunitionStatistics( group ) );

    private static EquipmentDefinition Weapon(
        string id,
        string name,
        int price,
        int bulk,
        int page,
        EquipmentWeaponCategory category,
        string group,
        EquipmentWeaponType type,
        int damageDie,
        string damageType,
        string hands,
        int? rangeFeet,
        IReadOnlyList<string>? traits = null,
        EquipmentRarity rarity = EquipmentRarity.Common ) => new EquipmentDefinition(
            $"equipment.{id}",
            name,
            EquipmentCategory.Weapon,
            rarity,
            price,
            bulk,
            1,
            new SourceReference( "Player Core", page ),
            new EquipmentWeaponStatistics( category, group, type, damageDie, damageType, hands, rangeFeet, traits ?? [] ) );

    private static EquipmentDefinition Armor(
        string id,
        string name,
        int price,
        int bulk,
        int page,
        EquipmentArmorCategory category,
        string group,
        int armorClassBonus,
        int dexterityCap,
        int checkPenalty,
        int speedPenaltyFeet,
        int strengthThreshold,
        IReadOnlyList<string>? traits = null ) => new EquipmentDefinition(
            $"equipment.{id}",
            name,
            EquipmentCategory.Armor,
            EquipmentRarity.Common,
            price,
            bulk,
            1,
            new SourceReference( "Player Core", page ),
            armor: new EquipmentArmorStatistics( category, group, armorClassBonus, dexterityCap, checkPenalty, speedPenaltyFeet, strengthThreshold, traits ?? [] ) );

    private static ClassKitDefinition Kit(
        string className,
        IReadOnlyList<ClassKitItem> items,
        IReadOnlyList<ClassKitOptionGroup> optionGroups,
        SourceReference source ) => new ClassKitDefinition(
            $"class_kit.{className}",
            $"class.{className}",
            $"{Char.ToUpperInvariant( className[ 0 ] )}{className[ 1.. ]} Kit",
            items,
            optionGroups,
            source );

    private static ClassKitItem I( string equipmentId, int quantity = 1 ) =>
        new ClassKitItem( $"equipment.{equipmentId}", quantity );

    private static ClassKitOption Option( string id, string name, params ClassKitItem[] items ) =>
        new ClassKitOption( id, name, items );

    private static ClassKitOption Dependency( string id, string name, ClassKitOptionDependency dependency ) =>
        new ClassKitOption( id, name, [], dependency );

    private static ClassKitOptionGroup Any( string id, params ClassKitOption[] options ) =>
        new ClassKitOptionGroup( id, ClassKitOptionSelection.Any, options );

    private static ClassKitOptionGroup AtMostOne( string id, params ClassKitOption[] options ) =>
        new ClassKitOptionGroup( id, ClassKitOptionSelection.AtMostOne, options );
}
