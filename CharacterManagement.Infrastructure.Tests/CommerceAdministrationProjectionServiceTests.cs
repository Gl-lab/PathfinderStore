using CharacterManagement.Infrastructure.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class CommerceAdministrationProjectionServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 27, 18, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task GameMasterProjectionsReturnWalletsContainersAndPublishedRevisions()
    {
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        DraftCharacter firstCharacter = DraftCharacter.Create(
            70,
            "Valeros",
            AncestryType.Human );
        DraftCharacter secondCharacter = DraftCharacter.Create(
            80,
            "Ezren",
            AncestryType.Human );
        characterDbContext.Character.AddRange( firstCharacter, secondCharacter );
        await characterDbContext.SaveChangesAsync();
        Campaign campaign = await AddCampaignAsync(
            campaignDbContext,
            firstCharacter.Id,
            secondCharacter.Id );
        Wallet wallet = Wallet.Create( campaign.Id, firstCharacter.Id, _now );
        wallet.ApplyAdjustment(
            Guid.NewGuid(),
            500,
            "Quest reward.",
            42,
            _now.AddMinutes( 1 ) );
        commerceDbContext.Wallets.Add( wallet );
        Settlement settlement = Settlement.Create(
            campaign.Id,
            "Otari",
            4,
            "Isle of Kortos",
            "Town",
            _now );
        Shop shop = settlement.AddShop(
            "Wrin's Wonders",
            "Curiosities",
            4,
            _now );
        commerceDbContext.Settlements.Add( settlement );
        await commerceDbContext.SaveChangesAsync();
        ItemConfiguration configuration = await AddPublishedItemAsync(
            catalogDbContext,
            campaign.Id,
            false,
            "item.healing-potion",
            "Healing Potion" );
        await AddPublishedItemAsync(
            catalogDbContext,
            campaign.Id,
            true,
            "equipment.spyglass",
            "Spyglass" );
        await AddPublishedItemAsync(
            catalogDbContext,
            campaign.Id + 1,
            false,
            "item.other-campaign",
            "Other Campaign Item" );
        InventoryContainer characterContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaign.Id,
            InventoryContainerOwnerKind.Character,
            firstCharacter.Id,
            _now );
        InventoryContainer shopContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaign.Id,
            InventoryContainerOwnerKind.Shop,
            shop.Id,
            _now );
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            campaign.Id,
            configuration.Id,
            characterContainer,
            "Valeros's Potion",
            _now );
        inventoryDbContext.AddRange( characterContainer, shopContainer, item );
        await inventoryDbContext.SaveChangesAsync();
        CommerceAdministrationProjectionService service = CreateService(
            campaignDbContext,
            characterDbContext,
            commerceDbContext,
            inventoryDbContext,
            catalogDbContext,
            true );

        IReadOnlyCollection<CommerceWalletSummaryDto> wallets =
            await service.GetWalletsAsync(
                campaign.Id,
                42,
                CancellationToken.None );
        WalletDto walletDetail = await service.GetWalletAsync(
            campaign.Id,
            firstCharacter.Id,
            42,
            CancellationToken.None );
        IReadOnlyCollection<InventoryContainerAdministrationDto> containers =
            await service.GetContainersAsync(
                campaign.Id,
                42,
                CancellationToken.None );
        IReadOnlyCollection<PublishedItemRevisionAdministrationDto> revisions =
            await service.SearchPublishedRevisionsAsync(
                campaign.Id,
                "potion",
                ItemCatalogScopeFilter.All,
                42,
                CancellationToken.None );
        IReadOnlyCollection<PublishedItemRevisionAdministrationDto> visibleRevisions =
            await service.SearchPublishedRevisionsAsync(
                campaign.Id,
                null,
                ItemCatalogScopeFilter.All,
                42,
                CancellationToken.None );

        Assert.Equal( 2, wallets.Count );
        Assert.Equal(
            500,
            wallets.Single( item =>
                item.CharacterId == firstCharacter.Id ).AvailableCopper );
        Assert.Equal(
            0,
            wallets.Single( item =>
                item.CharacterId == secondCharacter.Id ).AvailableCopper );
        Assert.Equal( "Quest reward.", Assert.Single( walletDetail.Entries ).Description );
        InventoryContainerAdministrationDto characterInventory = containers.Single(
            container =>
                container.ContainerKey == characterContainer.ContainerKey );
        Assert.Equal( "Valeros", characterInventory.OwnerName );
        Assert.Equal(
            "Valeros's Potion",
            Assert.Single( characterInventory.Items ).Name );
        Assert.Equal(
            "Wrin's Wonders",
            containers.Single( container =>
                container.ContainerKey == shopContainer.ContainerKey ).OwnerName );
        PublishedItemRevisionAdministrationDto revision = Assert.Single( revisions );
        Assert.Equal( "Healing Potion", revision.Name );
        Assert.Equal(
            configuration.Id,
            Assert.Single( revision.Configurations ).ItemConfigurationId );
        Assert.Equal( 2, visibleRevisions.Count );
        Assert.Contains( visibleRevisions, item => item.Name == "Spyglass" );
        Assert.DoesNotContain(
            visibleRevisions,
            item => item.Name == "Other Campaign Item" );
    }

    [Fact]
    public async Task SearchPublishedRevisionsIncludesLegacyAndExcludesForeignConfigurations()
    {
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _now );
        campaignDbContext.Campaigns.Add( campaign );
        await campaignDbContext.SaveChangesAsync();
        ItemConfiguration ownConfiguration = await AddPublishedItemAsync(
            catalogDbContext,
            campaign.Id,
            true,
            "item.healing-potion",
            "Healing Potion" );
        ItemConfiguration foreignConfiguration = ItemConfiguration.Create(
            campaign.Id + 1,
            ownConfiguration.ItemRevisionId,
            ItemSize.Large,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _now );
        ItemConfiguration legacyConfiguration = ItemConfiguration.Create(
            campaign.Id + 2,
            ownConfiguration.ItemRevisionId,
            ItemSize.Small,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _now );
        catalogDbContext.ItemConfigurations.AddRange(
            foreignConfiguration,
            legacyConfiguration );
        await catalogDbContext.SaveChangesAsync();
        catalogDbContext.Entry( legacyConfiguration )
            .Property( item => item.CampaignId )
            .CurrentValue = null;
        await catalogDbContext.SaveChangesAsync();
        CommerceAdministrationProjectionService service = CreateService(
            campaignDbContext,
            characterDbContext,
            commerceDbContext,
            inventoryDbContext,
            catalogDbContext,
            true );

        IReadOnlyCollection<PublishedItemRevisionAdministrationDto> revisions =
            await service.SearchPublishedRevisionsAsync(
                campaign.Id,
                null,
                ItemCatalogScopeFilter.All,
                42,
                CancellationToken.None );

        PublishedItemRevisionAdministrationDto revision = Assert.Single( revisions );
        int[] configurationIds = revision.Configurations
            .Select( configuration => configuration.ItemConfigurationId )
            .OrderBy( id => id )
            .ToArray();
        Assert.Contains( ownConfiguration.Id, configurationIds );
        Assert.Contains( legacyConfiguration.Id, configurationIds );
        Assert.DoesNotContain( foreignConfiguration.Id, configurationIds );
    }

    [Fact]
    public async Task AdministrationProjectionsRejectNonGameMaster()
    {
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        CommerceAdministrationProjectionService service = CreateService(
            campaignDbContext,
            characterDbContext,
            commerceDbContext,
            inventoryDbContext,
            catalogDbContext,
            false );

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetWalletsAsync(
                42,
                7,
                CancellationToken.None ) );
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetContainersAsync(
                42,
                7,
                CancellationToken.None ) );
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.SearchPublishedRevisionsAsync(
                42,
                null,
                ItemCatalogScopeFilter.All,
                7,
                CancellationToken.None ) );
    }

    private static async Task<Campaign> AddCampaignAsync(
        CampaignManagementDbContext dbContext,
        int firstCharacterId,
        int secondCharacterId )
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _now );
        dbContext.Campaigns.Add( campaign );
        await dbContext.SaveChangesAsync();
        CampaignInvitation firstInvitation = campaign.Invite(
            42,
            70,
            _now.AddSeconds( 1 ) );
        await dbContext.SaveChangesAsync();
        campaign.AcceptInvitation(
            firstInvitation.Id,
            70,
            _now.AddSeconds( 2 ) );
        CampaignInvitation secondInvitation = campaign.Invite(
            42,
            80,
            _now.AddSeconds( 3 ) );
        await dbContext.SaveChangesAsync();
        campaign.AcceptInvitation(
            secondInvitation.Id,
            80,
            _now.AddSeconds( 4 ) );
        campaign.CreateParty( 42, "Heroes", _now.AddSeconds( 5 ) );
        campaign.AssignCharacterToActiveParty(
            42,
            firstCharacterId,
            70,
            _now.AddSeconds( 6 ) );
        campaign.AssignCharacterToActiveParty(
            42,
            secondCharacterId,
            80,
            _now.AddSeconds( 7 ) );
        await dbContext.SaveChangesAsync();
        return campaign;
    }

    private static async Task<ItemConfiguration> AddPublishedItemAsync(
        ItemCatalogDbContext dbContext,
        int campaignId,
        bool isGlobal,
        string key,
        string name )
    {
        ItemDefinition definition = isGlobal
            ? ItemDefinition.CreateGlobal( key, _now )
            : ItemDefinition.CreateForCampaign( key, campaignId, _now );
        ItemRevision revision = definition.CreateRevision(
            name,
            $"{name} description.",
            1,
            400,
            1,
            ItemRevisionRules.Create(
                ItemCategory.OtherEquipment,
                equipment: EquipmentComponent.Create(
                    EquipmentUsage.Held,
                    1 ) ),
            _now );
        definition.PublishRevision( 1, _now );
        dbContext.ItemDefinitions.Add( definition );
        await dbContext.SaveChangesAsync();
        ItemConfiguration configuration = ItemConfiguration.Create(
            campaignId,
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _now );
        dbContext.ItemConfigurations.Add( configuration );
        await dbContext.SaveChangesAsync();
        return configuration;
    }

    private static CommerceAdministrationProjectionService CreateService(
        CampaignManagementDbContext campaignDbContext,
        CharacterManagementDbContext characterDbContext,
        CommerceDbContext commerceDbContext,
        InventoryDbContext inventoryDbContext,
        ItemCatalogDbContext catalogDbContext,
        bool isGameMaster )
    {
        return new CommerceAdministrationProjectionService(
            campaignDbContext,
            characterDbContext,
            commerceDbContext,
            inventoryDbContext,
            catalogDbContext,
            new InventoryItemCatalogProjectionReader( catalogDbContext ),
            new StubCommerceCampaignAccessPolicy( isGameMaster ) );
    }

    private static CampaignManagementDbContext CreateCampaignContext()
    {
        DbContextOptions<CampaignManagementDbContext> options =
            new DbContextOptionsBuilder<CampaignManagementDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CampaignManagementDbContext( options );
    }

    private static CommerceDbContext CreateCommerceContext()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CommerceDbContext( options );
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

    private sealed class StubCommerceCampaignAccessPolicy : ICommerceCampaignAccessPolicy
    {
        private readonly bool _isGameMaster;

        public StubCommerceCampaignAccessPolicy( bool isGameMaster )
        {
            _isGameMaster = isGameMaster;
        }

        public Task<bool> IsGameMasterAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( _isGameMaster );
        }
    }
}
