using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Configurations;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.ItemCatalog.Infrastructure.Tests;

public sealed class ItemConfigurationRepositoryTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 11, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task RepositoryPersistsImmutableConfigurationAndUpgrades()
    {
        DbContextOptions<ItemCatalogDbContext> options = new DbContextOptionsBuilder<ItemCatalogDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;
        string configurationKey;
        await using ( ItemCatalogDbContext writeContext = new ItemCatalogDbContext( options ) )
        {
            ItemDefinition definition = CreateDefinition();
            writeContext.ItemDefinitions.Add( definition );
            await writeContext.SaveChangesAsync();
            ItemRevision revision = Assert.Single( definition.Revisions );
            PermanentUpgrade upgrade = PermanentUpgrade.Create(
                "rune.striking",
                PermanentUpgradeKind.StrikingRune,
                1,
                PermanentUpgradeVisibility.Public );
            ItemConfiguration configuration = ItemConfiguration.Create(
                revision.Id,
                ItemSize.Medium,
                ItemMaterialType.ColdIron,
                ItemMaterialGrade.Standard,
                [ upgrade ],
                _createdAtUtc.AddMinutes( 2 ) );
            configurationKey = configuration.ConfigurationKey;
            writeContext.ItemConfigurations.Add( configuration );
            await writeContext.SaveChangesAsync();
        }

        await using ( ItemCatalogDbContext readContext = new ItemCatalogDbContext( options ) )
        {
            ItemConfigurationRepository repository = new ItemConfigurationRepository( readContext );
            ItemConfiguration? configuration = await repository.GetByConfigurationKeyAsync(
                configurationKey,
                CancellationToken.None );

            Assert.NotNull( configuration );
            Assert.Equal( ItemSize.Medium, configuration.Size );
            Assert.Equal( ItemMaterialType.ColdIron, configuration.MaterialType );
            PermanentUpgrade upgrade = Assert.Single( configuration.PermanentUpgrades );
            Assert.Equal( "rune.striking", upgrade.Code );
        }
    }

    private static ItemDefinition CreateDefinition()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            "equipment.longsword",
            _createdAtUtc );
        ItemRevisionRules rules = ItemRevisionRules.Create(
            ItemCategory.Weapon,
            attacks:
            [
                AttackComponent.Create(
                    "Blade",
                    1,
                    DamageDieSize.D8,
                    ItemDamageType.Slashing,
                    1 ),
            ],
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ),
            durability: DurabilityComponent.Create( 5, 20, 10 ) );
        definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            rules,
            _createdAtUtc.AddMinutes( 1 ) );
        return definition;
    }
}