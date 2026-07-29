using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Commerce;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.ItemCatalog.Infrastructure.Tests;

public sealed class CommerceCatalogReaderTests
{
    private const int OwnCampaignId = 42;
    private const int ForeignCampaignId = 77;

    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 11, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task IsPublishedConfigurationAcceptsOwnCampaignConfiguration()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        int configurationId = await SeedPublishedConfigurationAsync( options, OwnCampaignId );

        await using ItemCatalogDbContext context = new ItemCatalogDbContext( options );
        CommerceCatalogReader reader = new CommerceCatalogReader( context );

        bool isPublished = await reader.IsPublishedConfigurationAsync(
            configurationId,
            OwnCampaignId,
            CancellationToken.None );

        Assert.True( isPublished );
    }

    [Fact]
    public async Task IsPublishedConfigurationRejectsCrossCampaignConfiguration()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        int configurationId = await SeedPublishedConfigurationAsync( options, ForeignCampaignId );

        await using ItemCatalogDbContext context = new ItemCatalogDbContext( options );
        CommerceCatalogReader reader = new CommerceCatalogReader( context );

        bool isPublished = await reader.IsPublishedConfigurationAsync(
            configurationId,
            OwnCampaignId,
            CancellationToken.None );

        Assert.False( isPublished );
    }

    [Fact]
    public async Task IsPublishedConfigurationAcceptsLegacyNullCampaignConfiguration()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        int configurationId = await SeedPublishedConfigurationAsync(
            options,
            ForeignCampaignId,
            makeLegacy: true );

        await using ItemCatalogDbContext context = new ItemCatalogDbContext( options );
        CommerceCatalogReader reader = new CommerceCatalogReader( context );

        bool isPublished = await reader.IsPublishedConfigurationAsync(
            configurationId,
            OwnCampaignId,
            CancellationToken.None );

        Assert.True( isPublished );
    }

    [Fact]
    public async Task GetBasePriceCopperReturnsNullForCrossCampaignConfiguration()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        int configurationId = await SeedPublishedConfigurationAsync( options, ForeignCampaignId );

        await using ItemCatalogDbContext context = new ItemCatalogDbContext( options );
        CommerceCatalogReader reader = new CommerceCatalogReader( context );

        long? price = await reader.GetBasePriceCopperAsync(
            configurationId,
            OwnCampaignId,
            CancellationToken.None );

        Assert.Null( price );
    }

    [Fact]
    public async Task GetBasePriceCopperReturnsPriceForLegacyNullCampaignConfiguration()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        int configurationId = await SeedPublishedConfigurationAsync(
            options,
            ForeignCampaignId,
            makeLegacy: true );

        await using ItemCatalogDbContext context = new ItemCatalogDbContext( options );
        CommerceCatalogReader reader = new CommerceCatalogReader( context );

        long? price = await reader.GetBasePriceCopperAsync(
            configurationId,
            OwnCampaignId,
            CancellationToken.None );

        Assert.Equal( 100, price );
    }

    [Fact]
    public async Task GetRestockCandidatesKeepsLegacyAndOwnCampaignConfigurations()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        int ownConfigurationId = await SeedPublishedConfigurationAsync(
            options,
            OwnCampaignId,
            definitionKey: "equipment.longsword" );
        int legacyConfigurationId = await SeedPublishedConfigurationAsync(
            options,
            ForeignCampaignId,
            makeLegacy: true,
            definitionKey: "equipment.dagger" );
        int foreignConfigurationId = await SeedPublishedConfigurationAsync(
            options,
            ForeignCampaignId,
            definitionKey: "equipment.mace" );

        await using ItemCatalogDbContext context = new ItemCatalogDbContext( options );
        CommerceCatalogReader reader = new CommerceCatalogReader( context );

        IReadOnlyCollection<CommerceCatalogCandidate> candidates =
            await reader.GetRestockCandidatesAsync( OwnCampaignId, CancellationToken.None );

        int[] candidateIds = candidates
            .Select( candidate => candidate.ItemConfigurationId )
            .OrderBy( id => id )
            .ToArray();
        Assert.Contains( ownConfigurationId, candidateIds );
        Assert.Contains( legacyConfigurationId, candidateIds );
        Assert.DoesNotContain( foreignConfigurationId, candidateIds );
    }

    private static DbContextOptions<ItemCatalogDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<ItemCatalogDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;

    private static async Task<int> SeedPublishedConfigurationAsync(
        DbContextOptions<ItemCatalogDbContext> options,
        int configurationCampaignId,
        bool makeLegacy = false,
        string definitionKey = "equipment.longsword" )
    {
        await using ItemCatalogDbContext context = new ItemCatalogDbContext( options );
        ItemDefinition definition = CreateDefinition( definitionKey );
        definition.PublishRevision( 1, _createdAtUtc.AddMinutes( 2 ) );
        context.ItemDefinitions.Add( definition );
        await context.SaveChangesAsync();
        ItemRevision revision = Assert.Single( definition.Revisions );
        ItemConfiguration configuration = ItemConfiguration.Create(
            configurationCampaignId,
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _createdAtUtc.AddMinutes( 3 ) );
        context.ItemConfigurations.Add( configuration );
        await context.SaveChangesAsync();
        if ( makeLegacy )
        {
            context.Entry( configuration )
                .Property( item => item.CampaignId )
                .CurrentValue = null;
            await context.SaveChangesAsync();
        }

        return configuration.Id;
    }

    private static ItemDefinition CreateDefinition( string key )
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( key, _createdAtUtc );
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
            "Test item",
            String.Empty,
            0,
            100,
            1,
            rules,
            _createdAtUtc.AddMinutes( 1 ) );
        return definition;
    }
}