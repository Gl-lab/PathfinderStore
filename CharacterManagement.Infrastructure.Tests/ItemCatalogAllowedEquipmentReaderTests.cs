using CharacterManagement.Infrastructure.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class ItemCatalogAllowedEquipmentReaderTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 14, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task PublishedGlobalRevisionOverlaysStartingEquipmentDescription()
    {
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = CreateCharacterWithLongsword( characterDbContext );
        await using ItemCatalogDbContext itemCatalogDbContext = CreateItemCatalogContext();
        ItemDefinition definition = CreateGlobalWeapon(
            "Global Longsword",
            125,
            1.5m,
            DamageDieSize.D10 );
        itemCatalogDbContext.ItemDefinitions.Add( definition );
        await itemCatalogDbContext.SaveChangesAsync();
        ItemCatalogAllowedEquipmentReader reader = CreateReader( itemCatalogDbContext );

        AllowedEquipmentItem item = Assert.Single(
            reader.Read( character, [] ).Items );

        Assert.Equal( "Global Longsword", item.Name );
        Assert.Equal( 125, item.PriceCopper );
        Assert.Equal( 15, item.BulkTenths );
        Assert.Equal( 10, item.Weapon?.DamageDie );
    }

    [Fact]
    public async Task CampaignRevisionOverridesGlobalOnlyInsideExactCampaign()
    {
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = CreateCharacterWithLongsword( characterDbContext );
        await using ItemCatalogDbContext itemCatalogDbContext = CreateItemCatalogContext();
        ItemDefinition global = CreateGlobalWeapon(
            "Global Longsword",
            110,
            1,
            DamageDieSize.D10 );
        ItemDefinition campaign = CreateCampaignWeapon(
            42,
            "Campaign Longsword",
            120,
            2,
            DamageDieSize.D12 );
        itemCatalogDbContext.ItemDefinitions.AddRange( global, campaign );
        await itemCatalogDbContext.SaveChangesAsync();
        ItemCatalogAllowedEquipmentReader reader = CreateReader( itemCatalogDbContext );

        AllowedEquipmentItem campaignItem = Assert.Single(
            reader.Read( character, [], 42 ).Items );
        AllowedEquipmentItem otherCampaignItem = Assert.Single(
            reader.Read( character, [], 43 ).Items );

        Assert.Equal( "Campaign Longsword", campaignItem.Name );
        Assert.Equal( 12, campaignItem.Weapon?.DamageDie );
        Assert.Equal( "Global Longsword", otherCampaignItem.Name );
        Assert.Equal( 10, otherCampaignItem.Weapon?.DamageDie );
    }

    [Fact]
    public async Task DraftRevisionDoesNotReplaceStartingEquipmentFallback()
    {
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = CreateCharacterWithLongsword( characterDbContext );
        await using ItemCatalogDbContext itemCatalogDbContext = CreateItemCatalogContext();
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            "equipment.longsword",
            _createdAtUtc );
        definition.CreateRevision(
            "Unpublished Longsword",
            "Must not reach the combat card.",
            0,
            999,
            1,
            CreateWeaponRules( DamageDieSize.D12 ),
            _createdAtUtc.AddMinutes( 1 ) );
        itemCatalogDbContext.ItemDefinitions.Add( definition );
        await itemCatalogDbContext.SaveChangesAsync();
        ItemCatalogAllowedEquipmentReader reader = CreateReader( itemCatalogDbContext );

        AllowedEquipmentItem item = Assert.Single(
            reader.Read( character, [] ).Items );

        Assert.Equal( "Longsword", item.Name );
        Assert.Equal( 100, item.PriceCopper );
        Assert.Equal( 8, item.Weapon?.DamageDie );
    }

    [Fact]
    public async Task RetiredRevisionDoesNotReplaceStartingEquipmentFallback()
    {
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = CreateCharacterWithLongsword( characterDbContext );
        await using ItemCatalogDbContext itemCatalogDbContext = CreateItemCatalogContext();
        ItemDefinition definition = CreateGlobalWeapon(
            "Retired Longsword",
            999,
            1,
            DamageDieSize.D12 );
        definition.RetireRevision( 1, _createdAtUtc.AddMinutes( 3 ) );
        itemCatalogDbContext.ItemDefinitions.Add( definition );
        await itemCatalogDbContext.SaveChangesAsync();
        ItemCatalogAllowedEquipmentReader reader = CreateReader( itemCatalogDbContext );

        AllowedEquipmentItem item = Assert.Single(
            reader.Read( character, [] ).Items );

        Assert.Equal( "Longsword", item.Name );
        Assert.Equal( 100, item.PriceCopper );
        Assert.Equal( 8, item.Weapon?.DamageDie );
    }

    private static ItemCatalogAllowedEquipmentReader CreateReader(
        ItemCatalogDbContext itemCatalogDbContext )
    {
        return new ItemCatalogAllowedEquipmentReader(
            new EquipmentRepository(),
            itemCatalogDbContext );
    }

    private static DraftCharacter CreateCharacterWithLongsword(
        CharacterManagementDbContext dbContext )
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human );
        dbContext.Character.Add( character );
        dbContext.Entry( character )
            .Property( item => item.StartingEquipmentItems )
            .CurrentValue =
            [
                new CharacterEquipmentItem( "equipment.longsword", 1, 1 ),
            ];
        return character;
    }

    private static ItemDefinition CreateGlobalWeapon(
        string name,
        int price,
        decimal bulk,
        DamageDieSize damageDieSize )
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            "equipment.longsword",
            _createdAtUtc );
        AddPublishedRevision( definition, name, price, bulk, damageDieSize );
        return definition;
    }

    private static ItemDefinition CreateCampaignWeapon(
        int campaignId,
        string name,
        int price,
        decimal bulk,
        DamageDieSize damageDieSize )
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            "equipment.longsword",
            campaignId,
            _createdAtUtc );
        AddPublishedRevision( definition, name, price, bulk, damageDieSize );
        return definition;
    }

    private static void AddPublishedRevision(
        ItemDefinition definition,
        string name,
        int price,
        decimal bulk,
        DamageDieSize damageDieSize )
    {
        definition.CreateRevision(
            name,
            "Published catalog overlay.",
            0,
            price,
            bulk,
            CreateWeaponRules( damageDieSize ),
            _createdAtUtc.AddMinutes( 1 ) );
        definition.PublishRevision( 1, _createdAtUtc.AddMinutes( 2 ) );
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
}
