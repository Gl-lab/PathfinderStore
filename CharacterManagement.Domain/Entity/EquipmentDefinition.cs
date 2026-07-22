namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum EquipmentCategory
{
    Gear,
    Armor,
    Weapon,
    Ammunition,
    Shield
}

public enum EquipmentRarity
{
    Common,
    Uncommon
}

public enum EquipmentWeaponCategory
{
    Simple,
    Martial
}

public enum EquipmentWeaponType
{
    Melee,
    Ranged
}

public enum EquipmentArmorCategory
{
    Unarmored,
    Light,
    Medium
}

public enum ClassKitOptionSelection
{
    Any,
    AtMostOne
}

public enum ClassKitOptionDependency
{
    DeityFavoredWeapon
}

public sealed record EquipmentWeaponStatistics(
    EquipmentWeaponCategory Category,
    string Group,
    EquipmentWeaponType Type,
    int DamageDie,
    string DamageType,
    string Hands,
    int? RangeFeet,
    IReadOnlyList<string> Traits );

public sealed record EquipmentArmorStatistics(
    EquipmentArmorCategory Category,
    string Group,
    int ArmorClassBonus,
    int DexterityCap,
    int CheckPenalty,
    int SpeedPenaltyFeet,
    int StrengthThreshold,
    IReadOnlyList<string> Traits );

public sealed record EquipmentShieldStatistics(
    int ArmorClassBonus,
    int Hardness,
    int HitPoints );

public sealed record EquipmentAmmunitionStatistics( string WeaponGroup );

public sealed class EquipmentDefinition
{
    public string Id { get; }
    public string Name { get; }
    public EquipmentCategory Category { get; }
    public EquipmentRarity Rarity { get; }
    public int PriceCopper { get; }
    public int BulkTenths { get; }
    public int UnitsPerPurchase { get; }
    public SourceReference Source { get; }
    public EquipmentWeaponStatistics? Weapon { get; }
    public EquipmentArmorStatistics? Armor { get; }
    public EquipmentShieldStatistics? Shield { get; }
    public EquipmentAmmunitionStatistics? Ammunition { get; }

    public EquipmentDefinition(
        string id,
        string name,
        EquipmentCategory category,
        EquipmentRarity rarity,
        int priceCopper,
        int bulkTenths,
        int unitsPerPurchase,
        SourceReference source,
        EquipmentWeaponStatistics? weapon = null,
        EquipmentArmorStatistics? armor = null,
        EquipmentShieldStatistics? shield = null,
        EquipmentAmmunitionStatistics? ammunition = null )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "equipment.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Equipment id must use the 'equipment.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Equipment name cannot be empty.", nameof( name ) );
        }

        if ( priceCopper < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( priceCopper ) );
        }

        if ( bulkTenths < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( bulkTenths ) );
        }

        if ( unitsPerPurchase <= 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( unitsPerPurchase ) );
        }

        ArgumentNullException.ThrowIfNull( source );

        bool hasValidDetails = category switch
        {
            EquipmentCategory.Weapon => weapon is not null && armor is null && shield is null && ammunition is null,
            EquipmentCategory.Armor => weapon is null && armor is not null && shield is null && ammunition is null,
            EquipmentCategory.Shield => weapon is null && armor is null && shield is not null && ammunition is null,
            EquipmentCategory.Ammunition => weapon is null && armor is null && shield is null && ammunition is not null,
            _ => weapon is null && armor is null && shield is null && ammunition is null,
        };
        if ( !hasValidDetails )
        {
            throw new ArgumentException( "Equipment details must match its category." );
        }

        Id = id.Trim();
        Name = name.Trim();
        Category = category;
        Rarity = rarity;
        PriceCopper = priceCopper;
        BulkTenths = bulkTenths;
        UnitsPerPurchase = unitsPerPurchase;
        Source = source;
        Weapon = weapon;
        Armor = armor;
        Shield = shield;
        Ammunition = ammunition;
    }
}

public sealed record ClassKitItem( string EquipmentId, int PurchaseQuantity );

public sealed record ClassKitOption(
    string Id,
    string Name,
    IReadOnlyList<ClassKitItem> Items,
    ClassKitOptionDependency? Dependency = null );

public sealed record ClassKitOptionGroup(
    string Id,
    ClassKitOptionSelection Selection,
    IReadOnlyList<ClassKitOption> Options );

public sealed class ClassKitDefinition
{
    public const int StartingWealthCopper = 1500;

    public string Id { get; }
    public string CharacterClassId { get; }
    public string Name { get; }
    public IReadOnlyList<ClassKitItem> Items { get; }
    public IReadOnlyList<ClassKitOptionGroup> OptionGroups { get; }
    public SourceReference Source { get; }

    public ClassKitDefinition(
        string id,
        string characterClassId,
        string name,
        IReadOnlyList<ClassKitItem> items,
        IReadOnlyList<ClassKitOptionGroup> optionGroups,
        SourceReference source )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "class_kit.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Class kit id must use the 'class_kit.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( characterClassId ) ||
             !characterClassId.StartsWith( "class.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Class kit must reference a character class id.", nameof( characterClassId ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Class kit name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( items );
        ArgumentNullException.ThrowIfNull( optionGroups );
        ArgumentNullException.ThrowIfNull( source );

        if ( items.Count == 0 || items.Any( item => item.PurchaseQuantity <= 0 ) )
        {
            throw new ArgumentException( "Class kit must define valid base items.", nameof( items ) );
        }

        Id = id.Trim();
        CharacterClassId = characterClassId.Trim();
        Name = name.Trim();
        Items = items.ToArray();
        OptionGroups = optionGroups.ToArray();
        Source = source;
    }
}
