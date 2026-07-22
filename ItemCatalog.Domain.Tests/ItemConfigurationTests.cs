using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Exceptions;

namespace Pathfinder.ItemCatalog.Domain.Tests;

public sealed class ItemConfigurationTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 11, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateStoresSizeMaterialAndPermanentUpgrades()
    {
        PermanentUpgrade striking = PermanentUpgrade.Create(
            "rune.striking",
            PermanentUpgradeKind.StrikingRune,
            1,
            PermanentUpgradeVisibility.Public );
        PermanentUpgrade curse = PermanentUpgrade.Create(
            "effect.hungry-blade",
            PermanentUpgradeKind.TypedEffect,
            1,
            PermanentUpgradeVisibility.Hidden );

        ItemConfiguration configuration = ItemConfiguration.Create(
            17,
            ItemSize.Large,
            ItemMaterialType.ColdIron,
            ItemMaterialGrade.Standard,
            [ striking, curse ],
            _createdAtUtc );

        Assert.Equal( 17, configuration.ItemRevisionId );
        Assert.Equal( ItemSize.Large, configuration.Size );
        Assert.Equal( ItemMaterialType.ColdIron, configuration.MaterialType );
        Assert.Equal( ItemMaterialGrade.Standard, configuration.MaterialGrade );
        Assert.Equal( 2, configuration.PermanentUpgrades.Count );
        Assert.Equal( ItemConfiguration.ConfigurationKeyLength, configuration.ConfigurationKey.Length );
    }

    [Fact]
    public void ConfigurationKeyIsIndependentOfUpgradeInputOrder()
    {
        ItemConfiguration first = ItemConfiguration.Create(
            17,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            CreateUpgradePair( false ),
            _createdAtUtc );
        ItemConfiguration second = ItemConfiguration.Create(
            17,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            CreateUpgradePair( true ),
            _createdAtUtc );

        Assert.Equal( first.ConfigurationKey, second.ConfigurationKey );
    }

    [Fact]
    public void CreateRejectsDuplicateUpgradeCodes()
    {
        PermanentUpgrade first = PermanentUpgrade.Create(
            "rune.potency",
            PermanentUpgradeKind.WeaponPotencyRune,
            1,
            PermanentUpgradeVisibility.Public );
        PermanentUpgrade duplicate = PermanentUpgrade.Create(
            "rune.potency",
            PermanentUpgradeKind.WeaponPotencyRune,
            2,
            PermanentUpgradeVisibility.Public );

        Assert.Throws<ItemCatalogException>( () => ItemConfiguration.Create(
            17,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [ first, duplicate ],
            _createdAtUtc ) );
    }

    [Fact]
    public void UpgradeCannotBeReusedAcrossConfigurations()
    {
        PermanentUpgrade upgrade = PermanentUpgrade.Create(
            "rune.potency",
            PermanentUpgradeKind.WeaponPotencyRune,
            1,
            PermanentUpgradeVisibility.Public );
        ItemConfiguration.Create(
            17,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [ upgrade ],
            _createdAtUtc );

        Assert.Throws<ItemCatalogException>( () => ItemConfiguration.Create(
            18,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [ upgrade ],
            _createdAtUtc ) );
    }

    [Fact]
    public void FailedCreationDoesNotConsumeEarlierUnassignedUpgrade()
    {
        PermanentUpgrade available = PermanentUpgrade.Create(
            "rune.striking",
            PermanentUpgradeKind.StrikingRune,
            1,
            PermanentUpgradeVisibility.Public );
        PermanentUpgrade occupied = PermanentUpgrade.Create(
            "rune.potency",
            PermanentUpgradeKind.WeaponPotencyRune,
            1,
            PermanentUpgradeVisibility.Public );
        ItemConfiguration.Create(
            17,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [ occupied ],
            _createdAtUtc );

        Assert.Throws<ItemCatalogException>( () => ItemConfiguration.Create(
            18,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [ available, occupied ],
            _createdAtUtc ) );

        ItemConfiguration recovered = ItemConfiguration.Create(
            18,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [ available ],
            _createdAtUtc );
        Assert.Single( recovered.PermanentUpgrades );
    }

    private static IReadOnlyCollection<PermanentUpgrade> CreateUpgradePair( bool reversed )
    {
        PermanentUpgrade potency = PermanentUpgrade.Create(
            "rune.potency",
            PermanentUpgradeKind.WeaponPotencyRune,
            1,
            PermanentUpgradeVisibility.Public );
        PermanentUpgrade striking = PermanentUpgrade.Create(
            "rune.striking",
            PermanentUpgradeKind.StrikingRune,
            1,
            PermanentUpgradeVisibility.Public );
        return reversed ? [ striking, potency ] : [ potency, striking ];
    }
}