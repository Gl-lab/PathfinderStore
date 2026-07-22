using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Items;

public sealed class ItemRevision : Entity
{
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 4000;

    private ItemRevision()
    {
    }

    public int ItemDefinitionId { get; private set; }
    public int RevisionNumber { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public string Description { get; private set; } = String.Empty;
    public int Level { get; private set; }
    public int PriceInCopperPieces { get; private set; }
    public decimal Bulk { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    internal static ItemRevision Create(
        int revisionNumber,
        string name,
        string description,
        int level,
        int priceInCopperPieces,
        decimal bulk,
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

        return new ItemRevision
        {
            RevisionNumber = revisionNumber,
            Name = normalizedName,
            Description = normalizedDescription,
            Level = level,
            PriceInCopperPieces = priceInCopperPieces,
            Bulk = bulk,
            CreatedAtUtc = createdAtUtc,
        };
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
}