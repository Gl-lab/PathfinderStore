using Pathfinder.ItemCatalog.Domain.Exceptions;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class ItemRevisionRules
{
    private readonly List<AttackComponent> _attacks;
    private bool _isAssigned;

    private ItemRevisionRules(
        ItemCategory primaryCategory,
        IReadOnlyCollection<AttackComponent> attacks,
        ArmorComponent? armor,
        ShieldComponent? shield,
        EquipmentComponent? equipment,
        ConsumptionComponent? consumption,
        ChargeComponent? charges,
        DurabilityComponent? durability )
    {
        PrimaryCategory = primaryCategory;
        _attacks = attacks.ToList();
        Armor = armor;
        Shield = shield;
        Equipment = equipment;
        Consumption = consumption;
        Charges = charges;
        Durability = durability;
    }

    public ItemCategory PrimaryCategory { get; }
    public IReadOnlyList<AttackComponent> Attacks { get => _attacks.AsReadOnly(); }
    public ArmorComponent? Armor { get; }
    public ShieldComponent? Shield { get; }
    public EquipmentComponent? Equipment { get; }
    public ConsumptionComponent? Consumption { get; }
    public ChargeComponent? Charges { get; }
    public DurabilityComponent? Durability { get; }

    public static ItemRevisionRules Create(
        ItemCategory primaryCategory,
        IReadOnlyCollection<AttackComponent>? attacks = null,
        ArmorComponent? armor = null,
        ShieldComponent? shield = null,
        EquipmentComponent? equipment = null,
        ConsumptionComponent? consumption = null,
        ChargeComponent? charges = null,
        DurabilityComponent? durability = null )
    {
        if ( !Enum.IsDefined( primaryCategory ) )
        {
            throw new ItemCatalogException( "Primary item category is invalid." );
        }

        IReadOnlyCollection<AttackComponent> normalizedAttacks = attacks ?? [];
        bool hasAnyComponent = ( normalizedAttacks.Count > 0 ) || ( armor is not null ) ||
            ( shield is not null ) || ( equipment is not null ) ||
            ( consumption is not null ) || ( charges is not null ) ||
            ( durability is not null );
        if ( !hasAnyComponent )
        {
            throw new ItemCatalogException( "Item revision must contain at least one rules component." );
        }

        if ( shield is not null )
        {
            if ( ( normalizedAttacks.Count == 0 ) ||
                 ( equipment is null ) ||
                 ( durability is null ) )
            {
                throw new ItemCatalogException(
                    "Shield rules require attack, equipment, and durability components." );
            }
        }

        return new ItemRevisionRules(
            primaryCategory,
            normalizedAttacks,
            armor,
            shield,
            equipment,
            consumption,
            charges,
            durability );
    }

    internal void AssignToRevision()
    {
        if ( _isAssigned )
        {
            throw new ItemCatalogException(
                "The same rules component set cannot be assigned to multiple item revisions." );
        }

        _isAssigned = true;
    }
}