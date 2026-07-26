using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class UniqueItemAdministrationServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 26, 12, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task GameMasterCreatesCampaignScopedConfigurationAndInstanceIdempotently()
    {
        await using ItemCatalogDbContext itemCatalogDbContext = CreateItemCatalogContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        ItemDefinition definition = await AddPublishedDefinitionAsync(
            itemCatalogDbContext,
            42 );
        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.World,
            9,
            _now );
        inventoryDbContext.Containers.Add( container );
        await inventoryDbContext.SaveChangesAsync();
        UniqueItemAdministrationService service = CreateService(
            itemCatalogDbContext,
            inventoryDbContext,
            42,
            7 );
        Guid instanceKey = Guid.NewGuid();
        CreateUniqueItemRequest request = new CreateUniqueItemRequest(
            42,
            definition.Id,
            1,
            ItemSize.Medium,
            ItemMaterialType.ColdIron,
            ItemMaterialGrade.Standard,
            [
                PermanentUpgrade.Create(
                    "curse.binding",
                    PermanentUpgradeKind.TypedEffect,
                    1,
                    PermanentUpgradeVisibility.Hidden )
            ],
            instanceKey,
            container.ContainerKey,
            "Bound blade",
            7,
            Guid.NewGuid(),
            "story reward" );

        UniqueItemDto first = await service.CreateAsync(
            request,
            CancellationToken.None );
        UniqueItemDto replay = await service.CreateAsync(
            request,
            CancellationToken.None );

        Assert.Equal( first, replay );
        Assert.Equal( 42, Assert.Single( itemCatalogDbContext.ItemConfigurations ).CampaignId );
        ItemInstance instance = Assert.Single( inventoryDbContext.ItemInstances );
        Assert.True( instance.IsTransferRestricted );
        InventoryAuditEntry audit = Assert.Single( inventoryDbContext.AuditEntries );
        Assert.True( audit.IsForced );
        Assert.Equal( InventoryAuditActionKind.ForcedIssuance, audit.ActionKind );
        Assert.Equal( "story reward", audit.Reason );
        InventoryContainer destination = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.World,
            10,
            _now );
        Assert.Throws<InventoryException>( () =>
            instance.MoveTo(
                destination,
                "player-transfer",
                instance.Version,
                Guid.NewGuid(),
                "player",
                _now ) );
    }

    [Fact]
    public async Task GameMasterCannotUseDefinitionFromAnotherCampaign()
    {
        await using ItemCatalogDbContext itemCatalogDbContext = CreateItemCatalogContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        ItemDefinition definition = await AddPublishedDefinitionAsync(
            itemCatalogDbContext,
            43 );
        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.World,
            9,
            _now );
        inventoryDbContext.Containers.Add( container );
        await inventoryDbContext.SaveChangesAsync();
        UniqueItemAdministrationService service = CreateService(
            itemCatalogDbContext,
            inventoryDbContext,
            42,
            7 );

        await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
            service.CreateAsync(
                new CreateUniqueItemRequest(
                    42,
                    definition.Id,
                    1,
                    ItemSize.Medium,
                    ItemMaterialType.ColdIron,
                    ItemMaterialGrade.Standard,
                    [],
                    Guid.NewGuid(),
                    container.ContainerKey,
                    null,
                    7,
                    Guid.NewGuid(),
                    "test" ),
                CancellationToken.None ) );

        Assert.Empty( itemCatalogDbContext.ItemConfigurations );
        Assert.Empty( inventoryDbContext.ItemInstances );
    }

    private static UniqueItemAdministrationService CreateService(
        ItemCatalogDbContext itemCatalogDbContext,
        InventoryDbContext inventoryDbContext,
        int allowedCampaignId,
        int allowedUserId )
    {
        return new UniqueItemAdministrationService(
            itemCatalogDbContext,
            inventoryDbContext,
            new FakeAdministrativeAccess( allowedCampaignId, allowedUserId ),
            new ItemEffectRestrictionPolicy(),
            new FixedTimeProvider( _now ) );
    }

    private static async Task<ItemDefinition> AddPublishedDefinitionAsync(
        ItemCatalogDbContext dbContext,
        int campaignId )
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            "weapon.unique",
            campaignId,
            _now );
        definition.CreateRevision(
            "Unique weapon",
            "Campaign-authored weapon.",
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
        dbContext.ItemDefinitions.Add( definition );
        await dbContext.SaveChangesAsync();
        return definition;
    }

    private static ItemCatalogDbContext CreateItemCatalogContext()
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

    private sealed class FakeAdministrativeAccess : IItemCatalogAdministrativeAccess
    {
        private readonly int _allowedCampaignId;
        private readonly int _allowedUserId;

        public FakeAdministrativeAccess( int allowedCampaignId, int allowedUserId )
        {
            _allowedCampaignId = allowedCampaignId;
            _allowedUserId = allowedUserId;
        }

        public Task<bool> CanManageGlobalCatalogAsync(
            string userName,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( false );
        }

        public Task<bool> CanManageCampaignCatalogAsync(
            int userId,
            int campaignId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult(
                userId == _allowedUserId &&
                campaignId == _allowedCampaignId );
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _value;

        public FixedTimeProvider( DateTimeOffset value )
        {
            _value = value;
        }

        public override DateTimeOffset GetUtcNow() => _value;
    }
}