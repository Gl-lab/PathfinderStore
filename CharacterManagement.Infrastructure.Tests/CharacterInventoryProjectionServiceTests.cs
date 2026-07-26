using System.Text.Json;
using CharacterManagement.Infrastructure.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Access;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class CharacterInventoryProjectionServiceTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 26, 15, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task ProjectionReturnsExactRuntimeItemsAndServerComputedBulk()
    {
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext itemCatalogDbContext = CreateCatalogContext();
        DraftCharacter character = DraftCharacter.Create( 1, "Valeros", AncestryType.Human );
        character.FinalizeCreation( _createdAtUtc );
        characterDbContext.Character.Add( character );
        await characterDbContext.SaveChangesAsync();
        ItemConfiguration swordConfiguration = await AddCatalogItemAsync(
            itemCatalogDbContext,
            "equipment.longsword",
            "Longsword",
            ItemCategory.Weapon,
            1m );
        ItemConfiguration torchConfiguration = await AddCatalogItemAsync(
            itemCatalogDbContext,
            "equipment.torch",
            "Torch",
            ItemCategory.OtherEquipment,
            0.1m );
        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.Character,
            character.Id,
            _createdAtUtc );
        ItemInstance sword = ItemInstance.Create(
            Guid.NewGuid(),
            42,
            swordConfiguration.Id,
            container,
            null,
            _createdAtUtc );
        ItemInstance torches = ItemInstance.CreateStack(
            Guid.NewGuid(),
            42,
            torchConfiguration.Id,
            5,
            container,
            "Blue torches",
            _createdAtUtc.AddMinutes( 1 ) );
        inventoryDbContext.AddRange( container, sword, torches );
        await inventoryDbContext.SaveChangesAsync();
        character.SetRuntimeInventory(
        [
            new CharacterRuntimeEquipmentItem( sword.InstanceKey, true ),
            new CharacterRuntimeEquipmentItem( torches.InstanceKey, false ),
        ] );
        await characterDbContext.SaveChangesAsync();
        CharacterInventoryProjectionService service = new CharacterInventoryProjectionService(
            characterDbContext,
            inventoryDbContext,
            new InventoryItemCatalogProjectionReader( itemCatalogDbContext ),
            new FakeCharacterCampaignAccessPolicy(
                new CharacterCampaignAccess( true, true ) ) );

        CharacterInventoryDto result = await service.GetAsync(
            42,
            character.Id,
            7,
            CancellationToken.None );

        Assert.False( result.IsReadOnly );
        Assert.Equal( 2, result.Items.Count );
        CharacterInventoryItemDto swordDto = Assert.Single(
            result.Items,
            item => item.ItemInstanceKey == sword.InstanceKey );
        Assert.True( swordDto.IsEquipped );
        Assert.Equal( 0, swordDto.Version );
        Assert.Equal( "Longsword", swordDto.Revision.Name );
        Assert.Equal( 10, swordDto.Revision.BulkTenths );
        Assert.Equal( "StartingEquipment", swordDto.Provenance.Kind );
        CharacterInventoryItemDto torchDto = Assert.Single(
            result.Items,
            item => item.ItemInstanceKey == torches.InstanceKey );
        Assert.Equal( "Blue torches", torchDto.Revision.Name );
        Assert.Equal( 5, torchDto.Quantity );
        Assert.Equal( 15, result.Bulk.TotalTenths );
        Assert.Equal( 50, result.Bulk.EncumberedAtTenths );
        Assert.Equal( 100, result.Bulk.MaximumTenths );
    }

    [Fact]
    public async Task ProjectionDoesNotExposeHiddenConfigurationUpgrades()
    {
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext itemCatalogDbContext = CreateCatalogContext();
        DraftCharacter character = DraftCharacter.Create( 1, "Ezren", AncestryType.Human );
        characterDbContext.Character.Add( character );
        await characterDbContext.SaveChangesAsync();
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            "equipment.cursed-ring",
            42,
            _createdAtUtc );
        ItemRevision revision = definition.CreateRevision(
            "Plain ring",
            "A plain metal ring.",
            3,
            300,
            0.1m,
            ItemRevisionRules.Create(
                ItemCategory.OtherEquipment,
                equipment: EquipmentComponent.Create( EquipmentUsage.Worn, 0 ) ),
            _createdAtUtc );
        definition.PublishRevision( 1, _createdAtUtc );
        itemCatalogDbContext.ItemDefinitions.Add( definition );
        await itemCatalogDbContext.SaveChangesAsync();
        ItemConfiguration configuration = ItemConfiguration.Create(
            42,
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [
                PermanentUpgrade.Create(
                    "curse.binding",
                    PermanentUpgradeKind.TypedEffect,
                    1,
                    PermanentUpgradeVisibility.Hidden ),
            ],
            _createdAtUtc );
        itemCatalogDbContext.ItemConfigurations.Add( configuration );
        await itemCatalogDbContext.SaveChangesAsync();
        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.Character,
            character.Id,
            _createdAtUtc );
        ItemInstance instance = ItemInstance.Create(
            Guid.NewGuid(),
            42,
            configuration.Id,
            container,
            null,
            _createdAtUtc );
        inventoryDbContext.AddRange( container, instance );
        await inventoryDbContext.SaveChangesAsync();
        CharacterInventoryProjectionService service = new CharacterInventoryProjectionService(
            characterDbContext,
            inventoryDbContext,
            new InventoryItemCatalogProjectionReader( itemCatalogDbContext ),
            new FakeCharacterCampaignAccessPolicy(
                new CharacterCampaignAccess( true, false ) ) );

        CharacterInventoryDto result = await service.GetAsync(
            42,
            character.Id,
            7,
            CancellationToken.None );
        string json = JsonSerializer.Serialize( result );

        Assert.True( result.IsReadOnly );
        Assert.DoesNotContain( "curse.binding", json, StringComparison.Ordinal );
        Assert.Contains( "Plain ring", json, StringComparison.Ordinal );
    }

    [Fact]
    public async Task ProjectionRejectsUserWithoutCampaignCharacterAccess()
    {
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext itemCatalogDbContext = CreateCatalogContext();
        CharacterInventoryProjectionService service = new CharacterInventoryProjectionService(
            characterDbContext,
            inventoryDbContext,
            new InventoryItemCatalogProjectionReader( itemCatalogDbContext ),
            new FakeCharacterCampaignAccessPolicy( CharacterCampaignAccess.Denied ) );

        await Assert.ThrowsAsync<CharacterInventoryAccessDeniedException>(
            () => service.GetAsync( 42, 11, 7, CancellationToken.None ) );
    }

    private static async Task<ItemConfiguration> AddCatalogItemAsync(
        ItemCatalogDbContext dbContext,
        string key,
        string name,
        ItemCategory category,
        decimal bulk )
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            key,
            42,
            _createdAtUtc );
        ItemRevision revision = definition.CreateRevision(
            name,
            $"{name} description.",
            1,
            100,
            bulk,
            CreateRules( category, name ),
            _createdAtUtc );
        definition.PublishRevision( 1, _createdAtUtc );
        dbContext.ItemDefinitions.Add( definition );
        await dbContext.SaveChangesAsync();
        ItemConfiguration configuration = ItemConfiguration.Create(
            42,
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _createdAtUtc );
        dbContext.ItemConfigurations.Add( configuration );
        await dbContext.SaveChangesAsync();
        return configuration;
    }

    private static ItemRevisionRules CreateRules( ItemCategory category, string name )
    {
        return category == ItemCategory.Weapon
            ? ItemRevisionRules.Create(
                category,
                attacks:
                [
                    AttackComponent.Create(
                        name,
                        1,
                        DamageDieSize.D8,
                        ItemDamageType.Slashing,
                        1 ),
                ] )
            : ItemRevisionRules.Create(
                category,
                equipment: EquipmentComponent.Create( EquipmentUsage.Stored, 0 ) );
    }

    private static InventoryDbContext CreateInventoryContext()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new InventoryDbContext( options );
    }

    private static ItemCatalogDbContext CreateCatalogContext()
    {
        DbContextOptions<ItemCatalogDbContext> options =
            new DbContextOptionsBuilder<ItemCatalogDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new ItemCatalogDbContext( options );
    }

    private sealed class FakeCharacterCampaignAccessPolicy : ICharacterCampaignAccessPolicy
    {
        private readonly CharacterCampaignAccess _access;

        public FakeCharacterCampaignAccessPolicy( CharacterCampaignAccess access )
        {
            _access = access;
        }

        public Task<CharacterCampaignAccess> GetAccessAsync(
            int campaignId,
            int actingUserId,
            int characterId,
            CancellationToken cancellationToken ) => Task.FromResult( _access );
    }
}