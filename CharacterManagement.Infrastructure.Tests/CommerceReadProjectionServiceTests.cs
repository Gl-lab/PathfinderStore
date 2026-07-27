using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Domain.Transactions;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Commerce;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class CommerceReadProjectionServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 26, 17, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task WalletProjectionReturnsBalancesAndNewestLedgerFirst()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        Wallet wallet = Wallet.Create( 42, 101, _now );
        wallet.ApplyAdjustment(
            Guid.NewGuid(),
            1000,
            "Initial funds.",
            7,
            _now.AddMinutes( 1 ) );
        wallet.ApplyAdjustment(
            Guid.NewGuid(),
            -100,
            "Supplies.",
            7,
            _now.AddMinutes( 2 ) );
        commerceDbContext.Wallets.Add( wallet );
        await commerceDbContext.SaveChangesAsync();
        CommerceReadProjectionService service = CreateService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            catalogDbContext,
            new StubBuyerAccessPolicy( true ) );

        WalletDto result = await service.GetWalletAsync(
            42,
            101,
            7,
            CancellationToken.None );

        Assert.Equal( 900, result.BalanceCopper );
        Assert.Equal( 900, result.AvailableCopper );
        Assert.Equal( 2, result.Version );
        Assert.Equal(
            "Supplies.",
            result.Entries.First().Description );
    }

    [Fact]
    public async Task SettlementProjectionIsCampaignScopedAndIncludesShops()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _now );
        campaignDbContext.Campaigns.Add( campaign );
        await campaignDbContext.SaveChangesAsync();
        Settlement settlement = Settlement.Create(
            campaign.Id,
            "Otari",
            4,
            "Isle of Kortos",
            "Town",
            _now );
        settlement.AddShop(
            "Wrin's Wonders",
            "Curiosities",
            4,
            _now.AddMinutes( 1 ) );
        Settlement other = Settlement.Create(
            campaign.Id + 1,
            "Absalom",
            20,
            "Isle of Kortos",
            "Metropolis",
            _now );
        commerceDbContext.Settlements.AddRange( settlement, other );
        await commerceDbContext.SaveChangesAsync();
        CommerceReadProjectionService service = CreateService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            catalogDbContext,
            new StubBuyerAccessPolicy( false ) );

        IReadOnlyCollection<SettlementDto> result = await service.GetSettlementsAsync(
            campaign.Id,
            42,
            CancellationToken.None );

        SettlementDto dto = Assert.Single( result );
        Assert.Equal( "Otari", dto.Name );
        Assert.Equal( "Wrin's Wonders", Assert.Single( dto.Shops ).Name );
    }

    [Fact]
    public async Task OfferProjectionEnrichesCatalogDataAndFiltersActiveOffers()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _now );
        campaignDbContext.Campaigns.Add( campaign );
        await campaignDbContext.SaveChangesAsync();
        CampaignInvitation invitation = campaign.Invite(
            42,
            77,
            _now.AddMinutes( 1 ) );
        await campaignDbContext.SaveChangesAsync();
        campaign.AcceptInvitation(
            invitation.Id,
            77,
            _now.AddMinutes( 2 ) );
        await campaignDbContext.SaveChangesAsync();
        Shop shop = await AddShopAsync( commerceDbContext, campaign.Id );
        ItemConfiguration configuration = await AddCatalogItemAsync(
            catalogDbContext,
            campaign.Id,
            "item.healing-potion",
            "Healing Potion",
            1,
            400 );
        ShopOffer active = ShopOffer.CreateCatalog(
            campaign.Id,
            shop.Id,
            configuration.Id,
            3,
            450,
            _now );
        InventoryContainer shopContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaign.Id,
            InventoryContainerOwnerKind.Shop,
            shop.Id,
            _now );
        ItemInstance stockInstance = ItemInstance.Create(
            Guid.NewGuid(),
            campaign.Id,
            configuration.Id,
            shopContainer,
            "Wrin's Private Stock",
            _now );
        inventoryDbContext.Containers.Add( shopContainer );
        inventoryDbContext.ItemInstances.Add( stockInstance );
        await inventoryDbContext.SaveChangesAsync();
        ShopOffer stock = ShopOffer.CreateStockInstance(
            campaign.Id,
            shop.Id,
            stockInstance.InstanceKey,
            1,
            600,
            _now );
        ShopOffer soldOut = ShopOffer.CreateCatalog(
            campaign.Id,
            shop.Id,
            configuration.Id,
            1,
            450,
            _now.AddMinutes( 1 ) );
        soldOut.Reserve( 1 );
        soldOut.CompleteReserved( 1 );
        commerceDbContext.ShopOffers.AddRange( active, stock, soldOut );
        await commerceDbContext.SaveChangesAsync();
        CommerceReadProjectionService service = CreateService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            catalogDbContext,
            new StubBuyerAccessPolicy( false ) );

        IReadOnlyCollection<CommerceShopOfferDto> activeResult =
            await service.GetOffersAsync(
                campaign.Id,
                shop.Id,
                42,
                CommerceOfferStatusFilter.Active,
                CancellationToken.None );
        IReadOnlyCollection<CommerceShopOfferDto> allResult =
            await service.GetOffersAsync(
                campaign.Id,
                shop.Id,
                42,
                CommerceOfferStatusFilter.All,
                CancellationToken.None );

        CommerceShopOfferDto dto = activeResult.Single(
            offer => offer.Kind == ShopOfferKind.Catalog );
        Assert.Equal( "Healing Potion", dto.ItemName );
        Assert.Equal( 1, dto.ItemLevel );
        CommerceShopOfferDto stockDto = activeResult.Single(
            offer => offer.Kind == ShopOfferKind.StockInstance );
        Assert.Equal( "Wrin's Private Stock", stockDto.ItemName );
        Assert.Equal( 3, allResult.Count );
        IReadOnlyCollection<CommerceShopOfferDto> playerResult =
            await service.GetOffersAsync(
                campaign.Id,
                shop.Id,
                77,
                CommerceOfferStatusFilter.Active,
                CancellationToken.None );
        Assert.Equal( 2, playerResult.Count );
        await Assert.ThrowsAsync<CommerceReadAccessDeniedException>(
            () => service.GetOffersAsync(
                campaign.Id,
                shop.Id,
                77,
                CommerceOfferStatusFilter.All,
                CancellationToken.None ) );
    }

    [Fact]
    public async Task ReservationProjectionUsesEffectiveExpirationAndItemName()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        Shop shop = await AddShopAsync( commerceDbContext, 42 );
        ItemConfiguration configuration = await AddCatalogItemAsync(
            catalogDbContext,
            42,
            "item.scroll",
            "Scroll of Light",
            1,
            300 );
        ShopOffer offer = ShopOffer.CreateCatalog(
            42,
            shop.Id,
            configuration.Id,
            5,
            350,
            _now.AddMinutes( -10 ) );
        Guid operationId = Guid.NewGuid();
        PurchaseReservation reservation = PurchaseReservation.Create(
            operationId,
            42,
            offer.OfferKey,
            101,
            2,
            offer.UnitPriceCopper,
            _now.AddMinutes( -5 ),
            _now.AddMinutes( -1 ) );
        offer.Reserve( 2 );
        Wallet wallet = Wallet.Create( 42, 101, _now.AddMinutes( -6 ) );
        wallet.ApplyAdjustment(
            Guid.NewGuid(),
            1000,
            "Initial funds.",
            7,
            _now.AddMinutes( -6 ) );
        wallet.ReserveFunds(
            operationId,
            reservation.TotalPriceCopper,
            7,
            _now.AddMinutes( -5 ) );
        commerceDbContext.ShopOffers.Add( offer );
        commerceDbContext.PurchaseReservations.Add( reservation );
        commerceDbContext.Wallets.Add( wallet );
        await commerceDbContext.SaveChangesAsync();
        CommerceReadProjectionService service = CreateService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            catalogDbContext,
            new StubBuyerAccessPolicy( true ) );

        IReadOnlyCollection<CommercePurchaseReservationDto> result =
            await service.GetPurchaseReservationsAsync(
                42,
                101,
                7,
                PurchaseReservationStatus.Expired,
                CancellationToken.None );

        CommercePurchaseReservationDto dto = Assert.Single( result );
        Assert.Equal( "Scroll of Light", dto.ItemName );
        Assert.Equal( PurchaseReservationStatus.Expired, dto.Status );
        Assert.Equal( 700, dto.TotalPriceCopper );
        Assert.Equal( 0, wallet.ReservedCopper );
        Assert.Equal( 0, offer.ReservedQuantity );
    }

    [Fact]
    public async Task SellQuoteUsesOwnedTransferableItemAndCurrentShopPolicy()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        Shop shop = await AddShopAsync( commerceDbContext, 42 );
        shop.SetPricingPolicy( 100, 40 );
        await commerceDbContext.SaveChangesAsync();
        ItemConfiguration configuration = await AddCatalogItemAsync(
            catalogDbContext,
            42,
            "item.spyglass",
            "Spyglass",
            2,
            1000 );
        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.Character,
            101,
            _now );
        ItemInstance instance = ItemInstance.Create(
            Guid.NewGuid(),
            42,
            configuration.Id,
            container,
            "Captain's Spyglass",
            _now );
        inventoryDbContext.Containers.Add( container );
        inventoryDbContext.ItemInstances.Add( instance );
        await inventoryDbContext.SaveChangesAsync();
        CommerceReadProjectionService service = CreateService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            catalogDbContext,
            new StubBuyerAccessPolicy( true ) );

        CommerceSellQuoteDto result = await service.GetSellQuoteAsync(
            42,
            shop.Id,
            101,
            instance.InstanceKey,
            7,
            CancellationToken.None );

        Assert.Equal( "Captain's Spyglass", result.ItemName );
        Assert.Equal( 400, result.UnitPriceCopper );
        Assert.Equal( 400, result.TotalPriceCopper );
        await Assert.ThrowsAsync<CommerceReadNotFoundException>(
            () => service.GetSellQuoteAsync(
                42,
                shop.Id,
                202,
                instance.InstanceKey,
                7,
                CancellationToken.None ) );
    }

    [Fact]
    public async Task WalletProjectionRejectsUncontrolledCharacter()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        CommerceReadProjectionService service = CreateService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            catalogDbContext,
            new StubBuyerAccessPolicy( false ) );

        await Assert.ThrowsAsync<CommerceReadAccessDeniedException>(
            () => service.GetWalletAsync(
                42,
                101,
                7,
                CancellationToken.None ) );
    }

    [Fact]
    public async Task ControlledCharacterWithoutWalletReceivesZeroProjection()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        CommerceReadProjectionService service = CreateService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            catalogDbContext,
            new StubBuyerAccessPolicy( true ) );

        WalletDto result = await service.GetWalletAsync(
            42,
            101,
            7,
            CancellationToken.None );

        Assert.Equal( 0, result.AvailableCopper );
        Assert.Empty( result.Entries );
    }

    private static CommerceDbContext CreateCommerceContext()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CommerceDbContext( options );
    }

    private static CampaignManagementDbContext CreateCampaignContext()
    {
        DbContextOptions<CampaignManagementDbContext> options =
            new DbContextOptionsBuilder<CampaignManagementDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CampaignManagementDbContext( options );
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

    private static async Task<Shop> AddShopAsync(
        CommerceDbContext dbContext,
        int campaignId )
    {
        Settlement settlement = Settlement.Create(
            campaignId,
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
        dbContext.Settlements.Add( settlement );
        await dbContext.SaveChangesAsync();
        return shop;
    }

    private static async Task<ItemConfiguration> AddCatalogItemAsync(
        ItemCatalogDbContext dbContext,
        int campaignId,
        string key,
        string name,
        int level,
        int priceInCopperPieces )
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            key,
            campaignId,
            _now );
        ItemRevision revision = definition.CreateRevision(
            name,
            $"{name} description.",
            level,
            priceInCopperPieces,
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

    private static CommerceReadProjectionService CreateService(
        CommerceDbContext commerceDbContext,
        CampaignManagementDbContext campaignDbContext,
        InventoryDbContext inventoryDbContext,
        ItemCatalogDbContext catalogDbContext,
        ICommerceBuyerAccessPolicy buyerAccessPolicy ) =>
        new CommerceReadProjectionService(
            commerceDbContext,
            campaignDbContext,
            inventoryDbContext,
            new InventoryItemCatalogProjectionReader( catalogDbContext ),
            new CommerceCatalogReader( catalogDbContext ),
            buyerAccessPolicy,
            new FixedTimeProvider() );

    private sealed class StubBuyerAccessPolicy : ICommerceBuyerAccessPolicy
    {
        private readonly bool _controlsCharacter;

        public StubBuyerAccessPolicy( bool controlsCharacter )
        {
            _controlsCharacter = controlsCharacter;
        }

        public Task<bool> ControlsCharacterAsync(
            int campaignId,
            int actingUserId,
            int characterId,
            CancellationToken cancellationToken ) => Task.FromResult( _controlsCharacter );
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => _now;
    }
}
