using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Items;

public sealed class ItemRevision : Entity
{
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 4000;

    private ItemRevision()
    {
    }

    private readonly List<AttackComponent> _attacks = [];

    public int ItemDefinitionId { get; private set; }
    public int RevisionNumber { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public string Description { get; private set; } = String.Empty;
    public int Level { get; private set; }
    public int PriceInCopperPieces { get; private set; }
    public decimal Bulk { get; private set; }
    public ItemCategory PrimaryCategory { get; private set; }
    public ItemRevisionStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public DateTimeOffset? RetiredAtUtc { get; private set; }
    public IReadOnlyList<AttackComponent> Attacks { get => _attacks.AsReadOnly(); }
    public ArmorComponent? Armor { get; private set; }
    public ShieldComponent? Shield { get; private set; }
    public EquipmentComponent? Equipment { get; private set; }
    public ConsumptionComponent? Consumption { get; private set; }
    public ChargeComponent? Charges { get; private set; }
    public DurabilityComponent? Durability { get; private set; }

    internal static ItemRevision Create(
        int revisionNumber,
        string name,
        string description,
        int level,
        int priceInCopperPieces,
        decimal bulk,
        ItemRevisionRules rules,
        DateTimeOffset createdAtUtc )
    {
        string normalizedName = NormalizeRequiredText( name, "Revision name", NameMaxLength );
        string normalizedDescription = NormalizeOptionalText(
            description,
            "Revision description",
            DescriptionMaxLength );
        if ( revisionNumber <= 0 )
        {
            throw new ItemCatalogException( "Revision number must be greater than zero." );
        }

        if ( level < 0 )
        {
            throw new ItemCatalogException( "Item level cannot be negative." );
        }

        if ( priceInCopperPieces < 0 )
        {
            throw new ItemCatalogException( "Item price cannot be negative." );
        }

        if ( bulk < 0 )
        {
            throw new ItemCatalogException( "Item Bulk cannot be negative." );
        }

        ArgumentNullException.ThrowIfNull( rules );
        rules.AssignToRevision();

        ItemRevision revision = new ItemRevision
        {
            RevisionNumber = revisionNumber,
            Name = normalizedName,
            Description = normalizedDescription,
            Level = level,
            PriceInCopperPieces = priceInCopperPieces,
            Bulk = bulk,
            PrimaryCategory = rules.PrimaryCategory,
            Status = ItemRevisionStatus.Draft,
            CreatedAtUtc = createdAtUtc,
            Armor = rules.Armor,
            Shield = rules.Shield,
            Equipment = rules.Equipment,
            Consumption = rules.Consumption,
            Charges = rules.Charges,
            Durability = rules.Durability,
        };
        revision._attacks.AddRange( rules.Attacks );
        return revision;
    }

    internal void Publish( DateTimeOffset publishedAtUtc )
    {
        EnsureUtc( publishedAtUtc, "Publication timestamp" );
        if ( Status != ItemRevisionStatus.Draft )
        {
            throw new ItemCatalogException( "Only a draft item revision can be published." );
        }

        if ( publishedAtUtc < CreatedAtUtc )
        {
            throw new ItemCatalogException(
                "Publication timestamp cannot precede revision creation." );
        }

        Status = ItemRevisionStatus.Published;
        PublishedAtUtc = publishedAtUtc;
    }

    internal void Retire( DateTimeOffset retiredAtUtc )
    {
        EnsureUtc( retiredAtUtc, "Retirement timestamp" );
        if ( Status != ItemRevisionStatus.Published )
        {
            throw new ItemCatalogException( "Only a published item revision can be retired." );
        }

        if ( retiredAtUtc < PublishedAtUtc )
        {
            throw new ItemCatalogException(
                "Retirement timestamp cannot precede revision publication." );
        }

        Status = ItemRevisionStatus.Retired;
        RetiredAtUtc = retiredAtUtc;
    }

    private static string NormalizeRequiredText(
        string value,
        string fieldName,
        int maxLength )
    {
        if ( String.IsNullOrWhiteSpace( value ) )
        {
            throw new ItemCatalogException( $"{fieldName} cannot be empty." );
        }

        return NormalizeOptionalText( value, fieldName, maxLength );
    }

    private static string NormalizeOptionalText(
        string value,
        string fieldName,
        int maxLength )
    {
        string normalizedValue = value?.Trim() ?? String.Empty;
        if ( normalizedValue.Length > maxLength )
        {
            throw new ItemCatalogException( $"{fieldName} cannot exceed {maxLength} characters." );
        }

        return normalizedValue;
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new ItemCatalogException( $"{fieldName} must use UTC." );
        }
    }
}