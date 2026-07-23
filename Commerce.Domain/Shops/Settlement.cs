using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Shops;

public sealed class Settlement : Entity, IAggregateRoot
{
    public const int NameMaxLength = 200;
    public const int RegionMaxLength = 200;
    public const int TraitsMaxLength = 1000;

    private readonly List<Shop> _shops = [];

    private Settlement()
    {
    }

    public int CampaignId { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public int Level { get; private set; }
    public string Region { get; private set; } = String.Empty;
    public string Traits { get; private set; } = String.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<Shop> Shops { get => _shops; }

    public static Settlement Create(
        int campaignId,
        string name,
        int level,
        string region,
        string traits,
        DateTimeOffset createdAtUtc )
    {
        if ( campaignId <= 0 )
        {
            throw new CommerceException( "Campaign id must be greater than zero." );
        }

        if ( ( level < 0 ) || ( level > 20 ) )
        {
            throw new CommerceException( "Settlement level must be between 0 and 20." );
        }

        EnsureUtc( createdAtUtc );
        return new Settlement
        {
            CampaignId = campaignId,
            Name = NormalizeRequired( name, NameMaxLength, "Settlement name" ),
            Level = level,
            Region = NormalizeOptional( region, RegionMaxLength, "Settlement region" ),
            Traits = NormalizeOptional( traits, TraitsMaxLength, "Settlement traits" ),
            CreatedAtUtc = createdAtUtc,
        };
    }

    public Shop AddShop(
        string name,
        string specialization,
        int shopLevel,
        DateTimeOffset createdAtUtc )
    {
        if ( ( shopLevel < 0 ) || ( shopLevel > 20 ) )
        {
            throw new CommerceException( "Shop level must be between 0 and 20." );
        }

        EnsureUtc( createdAtUtc );
        string normalizedName = NormalizeRequired( name, Shop.NameMaxLength, "Shop name" );
        if ( _shops.Any( shop => String.Equals(
                 shop.Name,
                 normalizedName,
                 StringComparison.OrdinalIgnoreCase ) ) )
        {
            throw new CommerceException( "Settlement already contains a shop with this name." );
        }

        Shop shop = Shop.Create(
            CampaignId,
            normalizedName,
            specialization,
            shopLevel,
            createdAtUtc );
        _shops.Add( shop );
        return shop;
    }

    private static string NormalizeRequired( string value, int maxLength, string fieldName )
    {
        if ( String.IsNullOrWhiteSpace( value ) )
        {
            throw new CommerceException( $"{fieldName} cannot be empty." );
        }

        return NormalizeOptional( value, maxLength, fieldName );
    }

    private static string NormalizeOptional( string value, int maxLength, string fieldName )
    {
        string normalized = value?.Trim() ?? String.Empty;
        if ( normalized.Length > maxLength )
        {
            throw new CommerceException( $"{fieldName} cannot exceed {maxLength} characters." );
        }

        return normalized;
    }

    private static void EnsureUtc( DateTimeOffset value )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new CommerceException( "Creation timestamp must use UTC." );
        }
    }
}
