using System.Security.Cryptography;
using System.Text;
using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Configurations;

public sealed class ItemConfiguration : Entity, IAggregateRoot
{
    public const int ConfigurationKeyLength = 64;
    public const int MaximumUpgradeCount = 16;

    private readonly List<PermanentUpgrade> _permanentUpgrades = [];

    private ItemConfiguration()
    {
    }

    public int ItemRevisionId { get; private set; }
    public string ConfigurationKey { get; private set; } = String.Empty;
    public ItemSize Size { get; private set; }
    public ItemMaterialType MaterialType { get; private set; }
    public ItemMaterialGrade MaterialGrade { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyList<PermanentUpgrade> PermanentUpgrades { get => _permanentUpgrades.AsReadOnly(); }

    public static ItemConfiguration Create(
        int itemRevisionId,
        ItemSize size,
        ItemMaterialType materialType,
        ItemMaterialGrade materialGrade,
        IReadOnlyCollection<PermanentUpgrade>? permanentUpgrades,
        DateTimeOffset createdAtUtc )
    {
        if ( itemRevisionId <= 0 )
        {
            throw new ItemCatalogException( "Item revision id must be greater than zero." );
        }

        if ( !Enum.IsDefined( size ) ||
             !Enum.IsDefined( materialType ) ||
             !Enum.IsDefined( materialGrade ) )
        {
            throw new ItemCatalogException( "Item size or material is invalid." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new ItemCatalogException( "Configuration creation timestamp must use UTC." );
        }

        IReadOnlyCollection<PermanentUpgrade> upgrades = permanentUpgrades ?? [];
        if ( upgrades.Count > MaximumUpgradeCount )
        {
            throw new ItemCatalogException(
                $"Item configuration cannot exceed {MaximumUpgradeCount} permanent upgrades." );
        }

        if ( upgrades
            .GroupBy( upgrade => upgrade.Code )
            .Any( group => group.Count() > 1 ) )
        {
            throw new ItemCatalogException( "Permanent upgrade codes must be unique in a configuration." );
        }

        string configurationKey = CreateConfigurationKey(
            itemRevisionId,
            size,
            materialType,
            materialGrade,
            upgrades );
        ItemConfiguration configuration = new ItemConfiguration
        {
            ItemRevisionId = itemRevisionId,
            ConfigurationKey = configurationKey,
            Size = size,
            MaterialType = materialType,
            MaterialGrade = materialGrade,
            CreatedAtUtc = createdAtUtc,
        };
        foreach ( PermanentUpgrade upgrade in upgrades )
        {
            upgrade.EnsureCanAssignToConfiguration();
        }

        foreach ( PermanentUpgrade upgrade in upgrades )
        {
            upgrade.AssignToConfiguration();
            configuration._permanentUpgrades.Add( upgrade );
        }

        return configuration;
    }

    private static string CreateConfigurationKey(
        int itemRevisionId,
        ItemSize size,
        ItemMaterialType materialType,
        ItemMaterialGrade materialGrade,
        IReadOnlyCollection<PermanentUpgrade> upgrades )
    {
        IEnumerable<string> orderedUpgrades = upgrades
            .OrderBy( upgrade => upgrade.Code )
            .Select( upgrade =>
                $"{upgrade.Code}:{( int )upgrade.Kind}:{upgrade.Rank}:{( int )upgrade.Visibility}" );
        string canonicalValue = String.Join(
            '|',
            itemRevisionId,
            ( int )size,
            ( int )materialType,
            ( int )materialGrade,
            String.Join( ',', orderedUpgrades ) );
        byte[] hash = SHA256.HashData( Encoding.UTF8.GetBytes( canonicalValue ) );
        return Convert.ToHexString( hash )
            .ToLowerInvariant();
    }
}