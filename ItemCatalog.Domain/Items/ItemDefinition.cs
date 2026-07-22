using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Items;

public sealed class ItemDefinition : Entity, IAggregateRoot
{
    public const int KeyMaxLength = 200;

    private readonly List<ItemRevision> _revisions = [];

    private ItemDefinition()
    {
    }

    public string Key { get; private set; } = String.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyList<ItemRevision> Revisions { get => _revisions.AsReadOnly(); }

    public static ItemDefinition Create( string key, DateTimeOffset createdAtUtc )
    {
        string normalizedKey = NormalizeKey( key );
        EnsureUtc( createdAtUtc, "Creation timestamp" );

        return new ItemDefinition
        {
            Key = normalizedKey,
            CreatedAtUtc = createdAtUtc,
        };
    }

    public ItemRevision CreateRevision(
        string name,
        string description,
        int level,
        int priceInCopperPieces,
        decimal bulk,
        ItemRevisionRules rules,
        DateTimeOffset createdAtUtc )
    {
        EnsureUtc( createdAtUtc, "Revision creation timestamp" );
        if ( createdAtUtc < CreatedAtUtc )
        {
            throw new ItemCatalogException(
                "Revision creation timestamp cannot precede item definition creation." );
        }

        ItemRevision? previousRevision = _revisions.LastOrDefault();
        if ( ( previousRevision is not null ) &&
             ( createdAtUtc < previousRevision.CreatedAtUtc ) )
        {
            throw new ItemCatalogException(
                "Revision creation timestamp cannot precede the previous revision." );
        }

        ItemRevision revision = ItemRevision.Create(
            _revisions.Count + 1,
            name,
            description,
            level,
            priceInCopperPieces,
            bulk,
            rules,
            createdAtUtc );
        _revisions.Add( revision );
        return revision;
    }

    private static string NormalizeKey( string key )
    {
        if ( String.IsNullOrWhiteSpace( key ) )
        {
            throw new ItemCatalogException( "Item definition key cannot be empty." );
        }

        string normalizedKey = key.Trim();
        if ( normalizedKey.Length > KeyMaxLength )
        {
            throw new ItemCatalogException(
                $"Item definition key cannot exceed {KeyMaxLength} characters." );
        }

        string[] segments = normalizedKey.Split( '.' );
        bool hasInvalidSegment = segments.Any( segment =>
            ( segment.Length == 0 ) ||
            segment.Any( character =>
                !( Char.IsAsciiLetterLower( character ) ||
                   Char.IsAsciiDigit( character ) ||
                   ( character == '-' ) ) ) );
        if ( hasInvalidSegment )
        {
            throw new ItemCatalogException(
                "Item definition key must contain lowercase ASCII segments separated by dots." );
        }

        return normalizedKey;
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new ItemCatalogException( $"{fieldName} must use UTC." );
        }
    }
}