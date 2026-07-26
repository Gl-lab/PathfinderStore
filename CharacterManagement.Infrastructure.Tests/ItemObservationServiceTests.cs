using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class ItemObservationServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 26, 13, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task PlayerProjectionDoesNotSerializeHiddenUpgrade()
    {
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        Guid instanceKey = await AddItemAsync(
            catalogDbContext,
            inventoryDbContext );
        ItemObservationService service = new ItemObservationService(
            inventoryDbContext,
            catalogDbContext,
            new FakeObservationAccess( new ItemObservationAccess( true, false ) ) );

        ResolvedItemDto resolved = await service.ResolveAsync(
            42,
            instanceKey,
            CancellationToken.None );
        VisibleItemDto visible = await service.GetVisibleAsync(
            42,
            instanceKey,
            7,
            CancellationToken.None );
        string serialized = JsonSerializer.Serialize( visible );

        Assert.Equal( 2, resolved.PermanentUpgrades.Count );
        Assert.Equal( "rune.weapon-potency", Assert.Single( visible.PermanentUpgrades ).Code );
        Assert.DoesNotContain( "curse.binding", serialized, StringComparison.Ordinal );
        Assert.False( visible.IncludesHiddenProperties );
    }

    [Fact]
    public async Task GameMasterProjectionIncludesHiddenUpgrade()
    {
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        Guid instanceKey = await AddItemAsync(
            catalogDbContext,
            inventoryDbContext );
        ItemObservationService service = new ItemObservationService(
            inventoryDbContext,
            catalogDbContext,
            new FakeObservationAccess( new ItemObservationAccess( true, true ) ) );

        VisibleItemDto visible = await service.GetVisibleAsync(
            42,
            instanceKey,
            7,
            CancellationToken.None );

        Assert.Equal( 2, visible.PermanentUpgrades.Count );
        Assert.Contains(
            visible.PermanentUpgrades,
            item => item.Code == "curse.binding" );
        Assert.True( visible.IncludesHiddenProperties );
    }

    private static async Task<Guid> AddItemAsync(
        ItemCatalogDbContext catalogDbContext,
        InventoryDbContext inventoryDbContext )
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            "weapon.observed",
            42,
            _now );
        ItemRevision revision = definition.CreateRevision(
            "Observed blade",
            "A blade with a secret.",
            5,
            1000,
            1,
            ItemRevisionRules.Create(
                ItemCategory.Weapon,
                attacks:
                [
                    AttackComponent.Create(
                        "Blade",
                        1,
                        DamageDieSize.D8,
                        ItemDamageType.Slashing,
                        1,
                        null )
                ] ),
            _now );
        definition.PublishRevision( 1, _now );
        catalogDbContext.ItemDefinitions.Add( definition );
        await catalogDbContext.SaveChangesAsync();
        ItemConfiguration configuration = ItemConfiguration.Create(
            42,
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [
                PermanentUpgrade.Create(
                    "rune.weapon-potency",
                    PermanentUpgradeKind.WeaponPotencyRune,
                    1,
                    PermanentUpgradeVisibility.Public ),
                PermanentUpgrade.Create(
                    "curse.binding",
                    PermanentUpgradeKind.TypedEffect,
                    1,
                    PermanentUpgradeVisibility.Hidden )
            ],
            _now );
        catalogDbContext.ItemConfigurations.Add( configuration );
        await catalogDbContext.SaveChangesAsync();

        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.Character,
            11,
            _now );
        Guid instanceKey = Guid.NewGuid();
        ItemInstance instance = ItemInstance.Create(
            instanceKey,
            42,
            configuration.Id,
            container,
            null,
            _now );
        inventoryDbContext.Containers.Add( container );
        inventoryDbContext.ItemInstances.Add( instance );
        await inventoryDbContext.SaveChangesAsync();
        return instanceKey;
    }

    private static ItemCatalogDbContext CreateCatalogContext()
    {
        DbContextOptions<ItemCatalogDbContext> options =
            new DbContextOptionsBuilder<ItemCatalogDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new ItemCatalogDbContext( options );
    }

    private static InventoryDbContext CreateInventoryContext()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new InventoryDbContext( options );
    }

    private sealed class FakeObservationAccess : IItemObservationAccess
    {
        private readonly ItemObservationAccess _access;

        public FakeObservationAccess( ItemObservationAccess access )
        {
            _access = access;
        }

        public Task<ItemObservationAccess> GetAccessAsync(
            int campaignId,
            int observerUserId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( _access );
        }
    }
}