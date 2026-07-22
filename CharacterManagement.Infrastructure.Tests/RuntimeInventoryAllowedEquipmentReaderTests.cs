using CharacterManagement.Infrastructure.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class RuntimeInventoryAllowedEquipmentReaderTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 19, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task ReaderUsesExactRetiredRevisionReferencedByInstanceConfiguration()
    {
        await using CharacterManagementDbContext characterContext =
            TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human );
        character.FinalizeCreation( _createdAtUtc );
        characterContext.Character.Add( character );
        await characterContext.SaveChangesAsync();
        await using ItemCatalogDbContext catalogContext = CreateItemCatalogContext();
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            "equipment.longsword",
            _createdAtUtc );
        ItemRevision original = definition.CreateRevision(
            "Original Longsword",
            String.Empty,
            0,
            100,
            1,
            CreateWeaponRules( DamageDieSize.D8 ),
            _createdAtUtc );
        definition.PublishRevision( original.RevisionNumber, _createdAtUtc );
        catalogContext.ItemDefinitions.Add( definition );
        await catalogContext.SaveChangesAsync();
        ItemConfiguration configuration = ItemConfiguration.Create(
            original.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _createdAtUtc );
        catalogContext.ItemConfigurations.Add( configuration );
        ItemRevision current = definition.CreateRevision(
            "Current Longsword",
            String.Empty,
            0,
            200,
            2,
            CreateWeaponRules( DamageDieSize.D12 ),
            _createdAtUtc.AddMinutes( 1 ) );
        definition.PublishRevision( current.RevisionNumber, _createdAtUtc.AddMinutes( 1 ) );
        await catalogContext.SaveChangesAsync();
        await using InventoryDbContext inventoryContext = CreateInventoryContext();
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
        inventoryContext.Containers.Add( container );
        inventoryContext.ItemInstances.Add( instance );
        await inventoryContext.SaveChangesAsync();
        character.SetRuntimeInventory(
        [
            new CharacterRuntimeEquipmentItem( instance.InstanceKey, true ),
        ] );
        await characterContext.SaveChangesAsync();
        ItemCatalogAllowedEquipmentReader fallback = new ItemCatalogAllowedEquipmentReader(
            new EquipmentRepository(),
            catalogContext );
        RuntimeInventoryAllowedEquipmentReader reader = new RuntimeInventoryAllowedEquipmentReader(
            inventoryContext,
            catalogContext,
            fallback );

        AllowedEquipmentItem item = Assert.Single(
            reader.Read( character, [], 42 ).Items );

        Assert.Equal( "Original Longsword", item.Name );
        Assert.Equal( 100, item.PriceCopper );
        Assert.Equal( 8, item.Weapon?.DamageDie );
        Assert.Equal( 1, item.EquippedQuantity );
    }

    [Fact]
    public async Task ReaderRejectsInstanceOutsideCharacterContainer()
    {
        await using CharacterManagementDbContext characterContext =
            TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = DraftCharacter.Create( 1, "Valeros", AncestryType.Human );
        character.FinalizeCreation( _createdAtUtc );
        characterContext.Character.Add( character );
        await characterContext.SaveChangesAsync();
        await using ItemCatalogDbContext catalogContext = CreateItemCatalogContext();
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            "equipment.longsword",
            _createdAtUtc );
        ItemRevision revision = definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            CreateWeaponRules( DamageDieSize.D8 ),
            _createdAtUtc );
        definition.PublishRevision( 1, _createdAtUtc );
        catalogContext.ItemDefinitions.Add( definition );
        await catalogContext.SaveChangesAsync();
        ItemConfiguration configuration = ItemConfiguration.Create(
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _createdAtUtc );
        catalogContext.ItemConfigurations.Add( configuration );
        await catalogContext.SaveChangesAsync();
        await using InventoryDbContext inventoryContext = CreateInventoryContext();
        InventoryContainer foreignContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.Character,
            character.Id + 1,
            _createdAtUtc );
        ItemInstance instance = ItemInstance.Create(
            Guid.NewGuid(),
            42,
            configuration.Id,
            foreignContainer,
            null,
            _createdAtUtc );
        inventoryContext.AddRange( foreignContainer, instance );
        await inventoryContext.SaveChangesAsync();
        character.SetRuntimeInventory(
        [
            new CharacterRuntimeEquipmentItem( instance.InstanceKey, true ),
        ] );
        ItemCatalogAllowedEquipmentReader fallback = new ItemCatalogAllowedEquipmentReader(
            new EquipmentRepository(),
            catalogContext );
        RuntimeInventoryAllowedEquipmentReader reader = new RuntimeInventoryAllowedEquipmentReader(
            inventoryContext,
            catalogContext,
            fallback );

        Assert.Throws<InvalidOperationException>( () => reader.Read( character, [], 42 ) );
    }

    private static ItemRevisionRules CreateWeaponRules( DamageDieSize damageDieSize )
    {
        return ItemRevisionRules.Create(
            ItemCategory.Weapon,
            attacks:
            [
                AttackComponent.Create(
                    "Longsword",
                    1,
                    damageDieSize,
                    ItemDamageType.Slashing,
                    1 ),
            ] );
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
}
