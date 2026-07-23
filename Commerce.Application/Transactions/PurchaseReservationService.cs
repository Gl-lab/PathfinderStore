using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Transactions;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Transactions;

public sealed class PurchaseReservationService
{
    private static readonly TimeSpan ReservationLifetime = TimeSpan.FromMinutes( 15 );

    private readonly IPurchaseReservationRepository _repository;
    private readonly ICommerceBuyerAccessPolicy _accessPolicy;
    private readonly ICommerceInventoryTradePort _inventoryTradePort;
    private readonly ICommerceCatalogReader _catalogReader;
    private readonly TimeProvider _timeProvider;

    public PurchaseReservationService(
        IPurchaseReservationRepository repository,
        ICommerceBuyerAccessPolicy accessPolicy,
        ICommerceInventoryTradePort inventoryTradePort,
        ICommerceCatalogReader catalogReader,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _inventoryTradePort = inventoryTradePort;
        _catalogReader = catalogReader;
        _timeProvider = timeProvider;
    }

    public async Task<PurchaseReservationDto> CompleteAsync(
        int campaignId,
        Guid reservationKey,
        Guid operationId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        PurchaseReservation reservation = await _repository.GetAsync(
            campaignId,
            reservationKey,
            cancellationToken ) ?? throw new CommerceException( "Reservation was not found." );
        await EnsureControlsCharacterAsync(
            campaignId,
            actingUserId,
            reservation.BuyerCharacterId,
            cancellationToken );
        if ( reservation.Status == PurchaseReservationStatus.Completed )
        {
            return ToDto( reservation );
        }

        ShopOffer offer = await _repository.GetOfferAsync(
            campaignId,
            reservation.OfferKey,
            cancellationToken ) ?? throw new CommerceException( "Reserved offer was not found." );
        Wallet wallet = await _repository.GetWalletAsync(
            campaignId,
            reservation.BuyerCharacterId,
            cancellationToken ) ?? throw new CommerceException( "Buyer wallet was not found." );
        DateTimeOffset now = _timeProvider.GetUtcNow();
        if ( now > reservation.ExpiresAtUtc )
        {
            offer.Release( reservation.Quantity );
            if ( reservation.TotalPriceCopper > 0 )
            {
                wallet.ReleaseFunds(
                    operationId,
                    reservation.TotalPriceCopper,
                    actingUserId,
                    now );
            }

            reservation.Expire( now );
            await _repository.SaveChangesAsync( cancellationToken );
            return ToDto( reservation );
        }

        offer.CompleteReserved( reservation.Quantity );
        if ( reservation.TotalPriceCopper > 0 )
        {
            wallet.CompletePurchase(
                operationId,
                reservation.TotalPriceCopper,
                actingUserId,
                now );
        }

        Guid purchasedItemKey = await _inventoryTradePort.CompletePurchaseAsync(
            campaignId,
            offer.ShopId,
            reservation.BuyerCharacterId,
            offer.Kind,
            offer.ItemConfigurationId,
            offer.ItemInstanceKey,
            reservation.Quantity,
            operationId,
            actingUserId,
            now,
            cancellationToken );
        reservation.Complete( purchasedItemKey, now );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( reservation );
    }

    public async Task<ShopSaleDto> SellAsync(
        int campaignId,
        int shopId,
        int sellerCharacterId,
        Guid itemInstanceKey,
        Guid operationId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureControlsCharacterAsync(
            campaignId,
            actingUserId,
            sellerCharacterId,
            cancellationToken );
        ShopSale? existing = await _repository.GetSaleByOperationAsync(
            campaignId,
            operationId,
            cancellationToken );
        if ( existing is not null )
        {
            if ( ( existing.ShopId != shopId ) ||
                 ( existing.SellerCharacterId != sellerCharacterId ) ||
                 ( existing.ItemInstanceKey != itemInstanceKey ) )
            {
                throw new CommerceException(
                    "Sale operation id was already used with different data." );
            }

            return ToDto( existing );
        }

        Shop shop = await _repository.GetShopAsync(
            campaignId,
            shopId,
            cancellationToken ) ?? throw new CommerceException( "Campaign shop was not found." );
        CommerceSellableItem item = await _inventoryTradePort.GetSellableItemAsync(
            campaignId,
            sellerCharacterId,
            itemInstanceKey,
            operationId,
            actingUserId,
            cancellationToken ) ?? throw new CommerceException( "Seller item was not found." );
        if ( !item.CanTransfer )
        {
            throw new CommerceException( "Item cannot be sold in its current state." );
        }

        long basePrice = await _catalogReader.GetBasePriceCopperAsync(
            item.ItemConfigurationId,
            campaignId,
            cancellationToken ) ?? throw new CommerceException(
            "Item configuration is not available to this campaign." );
        long unitPrice = shop.CalculateBuybackPrice( basePrice );
        DateTimeOffset now = _timeProvider.GetUtcNow();
        ShopSale sale = ShopSale.Create(
            operationId,
            campaignId,
            shop.Id,
            sellerCharacterId,
            itemInstanceKey,
            item.ItemConfigurationId,
            item.Quantity,
            unitPrice,
            now );
        Wallet? wallet = await _repository.GetWalletAsync(
            campaignId,
            sellerCharacterId,
            cancellationToken );
        if ( wallet is null )
        {
            wallet = Wallet.Create( campaignId, sellerCharacterId, now );
            _repository.AddWallet( wallet );
        }

        if ( sale.TotalPriceCopper > 0 )
        {
            wallet.CreditSale( operationId, sale.TotalPriceCopper, actingUserId, now );
        }

        await _inventoryTradePort.MoveSaleToShopAsync(
            campaignId,
            shop.Id,
            item,
            operationId,
            actingUserId,
            now,
            cancellationToken );
        _repository.AddSale( sale );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( sale );
    }

    public async Task<PurchaseReservationDto> ReserveAsync(
        int campaignId,
        Guid operationId,
        Guid offerKey,
        int buyerCharacterId,
        int quantity,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureControlsCharacterAsync(
            campaignId,
            actingUserId,
            buyerCharacterId,
            cancellationToken );
        PurchaseReservation? existing = await _repository.GetByOperationAsync(
            campaignId,
            operationId,
            cancellationToken );
        if ( existing is not null )
        {
            EnsureSameRequest( existing, offerKey, buyerCharacterId, quantity );
            return ToDto( existing );
        }

        ShopOffer offer = await _repository.GetOfferAsync(
            campaignId,
            offerKey,
            cancellationToken ) ?? throw new CommerceException( "Active offer was not found." );
        Wallet wallet = await _repository.GetWalletAsync(
            campaignId,
            buyerCharacterId,
            cancellationToken ) ?? throw new CommerceException( "Buyer wallet was not found." );
        DateTimeOffset now = _timeProvider.GetUtcNow();
        PurchaseReservation reservation = PurchaseReservation.Create(
            operationId,
            campaignId,
            offerKey,
            buyerCharacterId,
            quantity,
            offer.UnitPriceCopper,
            now,
            now.Add( ReservationLifetime ) );
        offer.Reserve( quantity );
        if ( reservation.TotalPriceCopper > 0 )
        {
            wallet.ReserveFunds(
                operationId,
                reservation.TotalPriceCopper,
                actingUserId,
                now );
        }

        _repository.Add( reservation );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( reservation );
    }

    public async Task<PurchaseReservationDto> CancelAsync(
        int campaignId,
        Guid reservationKey,
        Guid releaseOperationId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        PurchaseReservation reservation = await _repository.GetAsync(
            campaignId,
            reservationKey,
            cancellationToken ) ?? throw new CommerceException( "Reservation was not found." );
        await EnsureControlsCharacterAsync(
            campaignId,
            actingUserId,
            reservation.BuyerCharacterId,
            cancellationToken );
        if ( reservation.Status == PurchaseReservationStatus.Cancelled )
        {
            return ToDto( reservation );
        }

        ShopOffer offer = await _repository.GetOfferAsync(
            campaignId,
            reservation.OfferKey,
            cancellationToken ) ?? throw new CommerceException( "Reserved offer was not found." );
        Wallet wallet = await _repository.GetWalletAsync(
            campaignId,
            reservation.BuyerCharacterId,
            cancellationToken ) ?? throw new CommerceException( "Buyer wallet was not found." );
        offer.Release( reservation.Quantity );
        if ( reservation.TotalPriceCopper > 0 )
        {
            wallet.ReleaseFunds(
                releaseOperationId,
                reservation.TotalPriceCopper,
                actingUserId,
                _timeProvider.GetUtcNow() );
        }

        reservation.Cancel( _timeProvider.GetUtcNow() );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( reservation );
    }

    private async Task EnsureControlsCharacterAsync(
        int campaignId,
        int actingUserId,
        int characterId,
        CancellationToken cancellationToken )
    {
        bool controlsCharacter = await _accessPolicy.ControlsCharacterAsync(
            campaignId,
            actingUserId,
            characterId,
            cancellationToken );
        if ( !controlsCharacter )
        {
            throw new UnauthorizedAccessException(
                "Only the active player controlling this character can reserve a purchase." );
        }
    }

    private static void EnsureSameRequest(
        PurchaseReservation existing,
        Guid offerKey,
        int buyerCharacterId,
        int quantity )
    {
        if ( ( existing.OfferKey != offerKey ) ||
             ( existing.BuyerCharacterId != buyerCharacterId ) ||
             ( existing.Quantity != quantity ) )
        {
            throw new CommerceException(
                "Purchase operation id was already used with different data." );
        }
    }

    private static PurchaseReservationDto ToDto( PurchaseReservation reservation ) =>
        new PurchaseReservationDto(
            reservation.ReservationKey,
            reservation.OperationId,
            reservation.CampaignId,
            reservation.OfferKey,
            reservation.BuyerCharacterId,
            reservation.Quantity,
            reservation.UnitPriceCopper,
            reservation.TotalPriceCopper,
            reservation.Status,
            reservation.ExpiresAtUtc,
            reservation.PurchasedItemInstanceKey );

    private static ShopSaleDto ToDto( ShopSale sale ) => new ShopSaleDto(
        sale.SaleKey,
        sale.OperationId,
        sale.CampaignId,
        sale.ShopId,
        sale.SellerCharacterId,
        sale.ItemInstanceKey,
        sale.Quantity,
        sale.UnitPriceCopper,
        sale.TotalPriceCopper,
        sale.CompletedAtUtc );
}
