using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Offers;

public sealed class ShopOfferAdministrationService
{
    private readonly IShopOfferRepository _repository;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;
    private readonly ICommerceCatalogReader _catalogReader;
    private readonly ICommerceInventoryReader _inventoryReader;
    private readonly TimeProvider _timeProvider;

    public ShopOfferAdministrationService(
        IShopOfferRepository repository,
        ICommerceCampaignAccessPolicy accessPolicy,
        ICommerceCatalogReader catalogReader,
        ICommerceInventoryReader inventoryReader,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _catalogReader = catalogReader;
        _inventoryReader = inventoryReader;
        _timeProvider = timeProvider;
    }

    public async Task<ShopOfferDto> CreateCatalogOfferAsync(
        int campaignId,
        int shopId,
        int itemConfigurationId,
        int quantity,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Shop shop = await GetAuthorizedShopAsync(
            campaignId,
            shopId,
            actingUserId,
            cancellationToken );
        bool isPublished = await _catalogReader.IsPublishedConfigurationAsync(
            itemConfigurationId,
            campaignId,
            cancellationToken );
        if ( !isPublished )
        {
            throw new CommerceException(
                "Catalog offer requires a published configuration available to this campaign." );
        }

        long basePrice = await _catalogReader.GetBasePriceCopperAsync(
            itemConfigurationId,
            campaignId,
            cancellationToken ) ?? throw new CommerceException(
            "Catalog configuration price was not found." );
        long unitPriceCopper = shop.CalculateCatalogPrice( basePrice );

        await _inventoryReader.EnsureShopContainerAsync(
            campaignId,
            shop.Id,
            cancellationToken );
        ShopOffer offer = ShopOffer.CreateCatalog(
            campaignId,
            shop.Id,
            itemConfigurationId,
            quantity,
            unitPriceCopper,
            _timeProvider.GetUtcNow() );
        _repository.Add( offer );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( offer );
    }

    public async Task<ShopOfferDto> CreateStockInstanceOfferAsync(
        int campaignId,
        int shopId,
        Guid itemInstanceKey,
        int quantity,
        long unitPriceCopper,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Shop shop = await GetAuthorizedShopAsync(
            campaignId,
            shopId,
            actingUserId,
            cancellationToken );
        CommerceStockItem item = await _inventoryReader.GetShopStockAsync(
            itemInstanceKey,
            cancellationToken ) ?? throw new CommerceException( "Stock item was not found." );
        if ( ( item.CampaignId != campaignId ) || ( item.OwnerShopId != shop.Id ) )
        {
            throw new CommerceException( "Stock item does not belong to this campaign shop." );
        }

        if ( !item.IsAvailable || ( quantity > item.Quantity ) )
        {
            throw new CommerceException( "Stock item is unavailable or has insufficient quantity." );
        }

        if ( await _repository.HasActiveInstanceOfferAsync(
                itemInstanceKey,
                cancellationToken ) )
        {
            throw new CommerceException( "Stock item already has an active offer." );
        }

        ShopOffer offer = ShopOffer.CreateStockInstance(
            campaignId,
            shop.Id,
            itemInstanceKey,
            quantity,
            unitPriceCopper,
            _timeProvider.GetUtcNow() );
        _repository.Add( offer );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( offer );
    }

    private async Task<Shop> GetAuthorizedShopAsync(
        int campaignId,
        int shopId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        bool isGameMaster = await _accessPolicy.IsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        if ( !isGameMaster )
        {
            throw new UnauthorizedAccessException(
                "Only an active campaign Game Master can manage offers." );
        }

        Shop shop = await _repository.GetShopAsync(
            shopId,
            cancellationToken ) ?? throw new CommerceException( "Shop was not found." );
        if ( shop.CampaignId != campaignId )
        {
            throw new CommerceException( "Shop does not belong to this campaign." );
        }

        return shop;
    }

    private static ShopOfferDto ToDto( ShopOffer offer ) => new ShopOfferDto(
        offer.OfferKey,
        offer.CampaignId,
        offer.ShopId,
        offer.Kind,
        offer.ItemConfigurationId,
        offer.ItemInstanceKey,
        offer.AvailableQuantity,
        offer.UnitPriceCopper,
        offer.Status );
}
