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
    public ItemCatalogScope Scope { get; private set; }
    public int? CampaignId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyList<ItemRevision> Revisions { get => _revisions.AsReadOnly(); }

    public static ItemDefinition CreateGlobal( string key, DateTimeOffset createdAtUtc )
    {
        return Create( key, ItemCatalogScope.Global, null, createdAtUtc );
    }

    public static ItemDefinition CreateForCampaign(
        string key,
        int campaignId,
        DateTimeOffset createdAtUtc )
    {
        if ( campaignId <= 0 )
        {
            throw new ItemCatalogException( "Campaign id must be greater than zero." );
        }

        return Create( key, ItemCatalogScope.Campaign, campaignId, createdAtUtc );
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

    private static ItemDefinition Create(
        string key,
        ItemCatalogScope scope,
        int? campaignId,
        DateTimeOffset createdAtUtc )
    {
        string normalizedKey = NormalizeKey( key );
        EnsureUtc( createdAtUtc, "Creation timestamp" );

        return new ItemDefinition
        {
            Key = normalizedKey,
            Scope = scope,
            CampaignId = campaignId,
            CreatedAtUtc = createdAtUtc,
        };
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new ItemCatalogException( $"{fieldName} must use UTC." );
        }
    }
}