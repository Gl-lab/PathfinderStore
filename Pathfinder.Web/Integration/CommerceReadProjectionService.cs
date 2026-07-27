using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Offers;
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

namespace Pathfinder.Web.Integration;

public sealed class CommerceReadProjectionService
{
    private readonly CommerceDbContext _commerceDbContext;
    private readonly CampaignManagementDbContext _campaignDbContext;
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly InventoryItemCatalogProjectionReader _catalogReader;
    private readonly ICommerceCatalogReader _commerceCatalogReader;
    private readonly ICommerceBuyerAccessPolicy _buyerAccessPolicy;
    private readonly TimeProvider _timeProvider;

    public CommerceReadProjectionService(
        CommerceDbContext commerceDbContext,
        CampaignManagementDbContext campaignDbContext,
        InventoryDbContext inventoryDbContext,
        InventoryItemCatalogProjectionReader catalogReader,
        ICommerceCatalogReader commerceCatalogReader,
        ICommerceBuyerAccessPolicy buyerAccessPolicy,
        TimeProvider timeProvider )
    {
        _commerceDbContext = commerceDbContext;
        _campaignDbContext = campaignDbContext;
        _inventoryDbContext = inventoryDbContext;
        _catalogReader = catalogReader;
        _commerceCatalogReader = commerceCatalogReader;
        _buyerAccessPolicy = buyerAccessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<WalletDto> GetWalletAsync(
        int campaignId,
        int characterId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        bool controlsCharacter = await _buyerAccessPolicy.ControlsCharacterAsync(
            campaignId,
            actingUserId,
            characterId,
            cancellationToken );
        if ( !controlsCharacter )
        {
            throw new CommerceReadAccessDeniedException();
        }

        Wallet? wallet = await _commerceDbContext.Wallets
            .AsNoTracking()
            .Include( item => item.Entries )
            .SingleOrDefaultAsync(
                item =>
                    item.CampaignId == campaignId &&
                    item.CharacterId == characterId,
                cancellationToken );
        if ( wallet is null )
        {
            return new WalletDto(
                campaignId,
                characterId,
                0,
                0,
                0,
                0,
                [] );
        }

        WalletLedgerEntryDto[] entries = wallet.Entries
            .OrderByDescending( entry => entry.OccurredAtUtc )
            .ThenBy( entry => entry.OperationId )
            .Select( entry => new WalletLedgerEntryDto(
                entry.OperationId,
                entry.Kind,
                entry.AmountCopper,
                entry.BalanceAfterCopper,
                entry.Description,
                entry.PerformedByUserId,
                entry.OccurredAtUtc ) )
            .ToArray();
        return new WalletDto(
            wallet.CampaignId,
            wallet.CharacterId,
            wallet.BalanceCopper,
            wallet.ReservedCopper,
            wallet.AvailableCopper,
            wallet.Version,
            entries );
    }

    public async Task<IReadOnlyCollection<SettlementDto>> GetSettlementsAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _campaignDbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Memberships )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == campaignId &&
                    item.Status == CampaignStatus.Active,
                cancellationToken );
        bool isMember = campaign is not null &&
            ( campaign.HasActiveRole( actingUserId, CampaignMembershipRole.Player ) ||
              campaign.HasActiveRole( actingUserId, CampaignMembershipRole.GameMaster ) );
        if ( !isMember )
        {
            throw new CommerceReadAccessDeniedException();
        }

        Settlement[] settlements = await _commerceDbContext.Settlements
            .AsNoTracking()
            .Include( settlement => settlement.Shops )
            .Where( settlement => settlement.CampaignId == campaignId )
            .OrderBy( settlement => settlement.Name )
            .ThenBy( settlement => settlement.Id )
            .ToArrayAsync( cancellationToken );
        return settlements
            .Select( ToDto )
            .ToArray();
    }

    public async Task<IReadOnlyCollection<CommerceShopOfferDto>> GetOffersAsync(
        int campaignId,
        int shopId,
        int actingUserId,
        CommerceOfferStatusFilter status,
        CancellationToken cancellationToken )
    {
        bool isGameMaster = await EnsureCampaignMemberAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        if ( status == CommerceOfferStatusFilter.All && !isGameMaster )
        {
            throw new CommerceReadAccessDeniedException();
        }

        bool shopExists = await _commerceDbContext.Shops
            .AsNoTracking()
            .AnyAsync(
                shop =>
                    shop.Id == shopId &&
                    shop.CampaignId == campaignId,
                cancellationToken );
        if ( !shopExists )
        {
            throw new CommerceReadNotFoundException();
        }

        IQueryable<ShopOffer> query = _commerceDbContext.ShopOffers
            .AsNoTracking()
            .Where( offer =>
                offer.CampaignId == campaignId &&
                offer.ShopId == shopId );
        if ( status == CommerceOfferStatusFilter.Active )
        {
            query = query.Where( offer => offer.Status == ShopOfferStatus.Active );
        }

        ShopOffer[] offers = await query
            .OrderBy( offer => offer.CreatedAtUtc )
            .ThenBy( offer => offer.OfferKey )
            .ToArrayAsync( cancellationToken );
        Dictionary<Guid, ItemInstance> stockInstances = await ReadStockInstancesAsync(
            campaignId,
            offers,
            cancellationToken );
        int[] configurationIds = offers
            .Select( offer => ResolveConfigurationId( offer, stockInstances ) )
            .Distinct()
            .ToArray();
        Dictionary<int, InventoryItemCatalogProjection> catalog =
            await _catalogReader.ReadAsync(
                campaignId,
                configurationIds,
                cancellationToken );
        return offers
            .Select( offer =>
            {
                int configurationId = ResolveConfigurationId( offer, stockInstances );
                InventoryItemCatalogProjection item = catalog[ configurationId ];
                string name = offer.ItemInstanceKey is Guid instanceKey
                    ? stockInstances[ instanceKey ].CustomName ?? item.Name
                    : item.Name;
                return new CommerceShopOfferDto(
                    offer.OfferKey,
                    offer.CampaignId,
                    offer.ShopId,
                    offer.Kind,
                    configurationId,
                    offer.ItemInstanceKey,
                    name,
                    item.Level,
                    offer.AvailableQuantity,
                    offer.ReservedQuantity,
                    offer.UnitPriceCopper,
                    offer.Status,
                    offer.Version );
            } )
            .ToArray();
    }

    public async Task<IReadOnlyCollection<CommercePurchaseReservationDto>>
        GetPurchaseReservationsAsync(
            int campaignId,
            int buyerCharacterId,
            int actingUserId,
            PurchaseReservationStatus? status,
            CancellationToken cancellationToken )
    {
        await EnsureControlsCharacterAsync(
            campaignId,
            buyerCharacterId,
            actingUserId,
            cancellationToken );
        DateTimeOffset now = _timeProvider.GetUtcNow();
        await ExpirePurchaseReservationsAsync(
            campaignId,
            buyerCharacterId,
            actingUserId,
            now,
            cancellationToken );
        PurchaseReservation[] reservations = await _commerceDbContext.PurchaseReservations
            .AsNoTracking()
            .Where( reservation =>
                reservation.CampaignId == campaignId &&
                reservation.BuyerCharacterId == buyerCharacterId )
            .OrderByDescending( reservation => reservation.CreatedAtUtc )
            .ThenBy( reservation => reservation.ReservationKey )
            .ToArrayAsync( cancellationToken );
        Guid[] offerKeys = reservations
            .Select( reservation => reservation.OfferKey )
            .Distinct()
            .ToArray();
        ShopOffer[] offers = await _commerceDbContext.ShopOffers
            .AsNoTracking()
            .Where( offer =>
                offer.CampaignId == campaignId &&
                offerKeys.Contains( offer.OfferKey ) )
            .ToArrayAsync( cancellationToken );
        Dictionary<Guid, ItemInstance> stockInstances = await ReadStockInstancesAsync(
            campaignId,
            offers,
            cancellationToken );
        Dictionary<Guid, ShopOffer> offersByKey = offers
            .ToDictionary( offer => offer.OfferKey );
        int[] configurationIds = offers
            .Select( offer => ResolveConfigurationId( offer, stockInstances ) )
            .Distinct()
            .ToArray();
        Dictionary<int, InventoryItemCatalogProjection> catalog =
            await _catalogReader.ReadAsync(
                campaignId,
                configurationIds,
                cancellationToken );
        return reservations
            .Select( reservation =>
            {
                ShopOffer offer = offersByKey[ reservation.OfferKey ];
                int configurationId = ResolveConfigurationId( offer, stockInstances );
                InventoryItemCatalogProjection item = catalog[ configurationId ];
                string name = offer.ItemInstanceKey is Guid instanceKey
                    ? stockInstances[ instanceKey ].CustomName ?? item.Name
                    : item.Name;
                PurchaseReservationStatus effectiveStatus =
                    reservation.Status == PurchaseReservationStatus.Active &&
                    reservation.ExpiresAtUtc <= now
                        ? PurchaseReservationStatus.Expired
                        : reservation.Status;
                return new CommercePurchaseReservationDto(
                    reservation.ReservationKey,
                    reservation.OperationId,
                    reservation.CampaignId,
                    reservation.OfferKey,
                    reservation.BuyerCharacterId,
                    name,
                    reservation.Quantity,
                    reservation.UnitPriceCopper,
                    reservation.TotalPriceCopper,
                    effectiveStatus,
                    reservation.ExpiresAtUtc,
                    reservation.PurchasedItemInstanceKey );
            } )
            .Where( reservation =>
                status is null ||
                reservation.Status == status )
            .ToArray();
    }

    public async Task<CommerceSellQuoteDto> GetSellQuoteAsync(
        int campaignId,
        int shopId,
        int sellerCharacterId,
        Guid itemInstanceKey,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureControlsCharacterAsync(
            campaignId,
            sellerCharacterId,
            actingUserId,
            cancellationToken );
        Shop shop = await _commerceDbContext.Shops
            .AsNoTracking()
            .SingleOrDefaultAsync(
                shopEntity =>
                    shopEntity.Id == shopId &&
                    shopEntity.CampaignId == campaignId,
                cancellationToken )
            ?? throw new CommerceReadNotFoundException();
        ItemInstance instance = await (
            from item in _inventoryDbContext.ItemInstances.AsNoTracking()
            join container in _inventoryDbContext.Containers.AsNoTracking()
                on item.CurrentContainerKey equals container.ContainerKey
            where
                item.InstanceKey == itemInstanceKey &&
                item.CampaignId == campaignId &&
                item.Quantity > 0 &&
                item.ReservationKey == null &&
                !item.IsTransferRestricted &&
                item.AttachedToInstanceKey == null &&
                container.CampaignId == campaignId &&
                container.OwnerKind == InventoryContainerOwnerKind.Character &&
                container.OwnerId == sellerCharacterId
            select item )
            .SingleOrDefaultAsync( cancellationToken )
            ?? throw new CommerceReadNotFoundException();
        Dictionary<int, InventoryItemCatalogProjection> catalog =
            await _catalogReader.ReadAsync(
                campaignId,
                [ instance.ItemConfigurationId ],
                cancellationToken );
        InventoryItemCatalogProjection catalogItem =
            catalog[ instance.ItemConfigurationId ];
        long basePriceCopper = await _commerceCatalogReader.GetBasePriceCopperAsync(
            instance.ItemConfigurationId,
            campaignId,
            cancellationToken ) ?? throw new CommerceReadNotFoundException();
        long unitPriceCopper = shop.CalculateBuybackPrice( basePriceCopper );
        return new CommerceSellQuoteDto(
            instance.InstanceKey,
            instance.ItemConfigurationId,
            instance.CustomName ?? catalogItem.Name,
            instance.Quantity,
            unitPriceCopper,
            checked( unitPriceCopper * instance.Quantity ) );
    }

    private async Task<bool> EnsureCampaignMemberAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _campaignDbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Memberships )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == campaignId &&
                    item.Status == CampaignStatus.Active,
                cancellationToken );
        bool isGameMaster = campaign is not null &&
            campaign.HasActiveRole( actingUserId, CampaignMembershipRole.GameMaster );
        bool isMember = isGameMaster ||
            ( campaign is not null &&
              campaign.HasActiveRole( actingUserId, CampaignMembershipRole.Player ) );
        if ( !isMember )
        {
            throw new CommerceReadAccessDeniedException();
        }

        return isGameMaster;
    }

    private async Task ExpirePurchaseReservationsAsync(
        int campaignId,
        int buyerCharacterId,
        int actingUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken )
    {
        PurchaseReservation[] expired = await _commerceDbContext.PurchaseReservations
            .Where( reservation =>
                reservation.CampaignId == campaignId &&
                reservation.BuyerCharacterId == buyerCharacterId &&
                reservation.Status == PurchaseReservationStatus.Active &&
                reservation.ExpiresAtUtc <= now )
            .ToArrayAsync( cancellationToken );
        if ( expired.Length == 0 )
        {
            return;
        }

        Guid[] offerKeys = expired
            .Select( reservation => reservation.OfferKey )
            .Distinct()
            .ToArray();
        Dictionary<Guid, ShopOffer> offers = await _commerceDbContext.ShopOffers
            .Where( offer =>
                offer.CampaignId == campaignId &&
                offerKeys.Contains( offer.OfferKey ) )
            .ToDictionaryAsync( offer => offer.OfferKey, cancellationToken );
        Wallet wallet = await _commerceDbContext.Wallets
            .Include( item => item.Entries )
            .SingleAsync(
                item =>
                    item.CampaignId == campaignId &&
                    item.CharacterId == buyerCharacterId,
                cancellationToken );
        foreach ( PurchaseReservation reservation in expired )
        {
            offers[ reservation.OfferKey ].Release( reservation.Quantity );
            if ( reservation.TotalPriceCopper > 0 )
            {
                wallet.ReleaseFunds(
                    reservation.ReservationKey,
                    reservation.TotalPriceCopper,
                    actingUserId,
                    now );
            }

            reservation.Expire( now );
        }

        try
        {
            await _commerceDbContext.SaveChangesAsync( cancellationToken );
        }
        catch ( DbUpdateConcurrencyException )
        {
            _commerceDbContext.ChangeTracker.Clear();
        }
    }

    private async Task EnsureControlsCharacterAsync(
        int campaignId,
        int characterId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        bool controlsCharacter = await _buyerAccessPolicy.ControlsCharacterAsync(
            campaignId,
            actingUserId,
            characterId,
            cancellationToken );
        if ( !controlsCharacter )
        {
            throw new CommerceReadAccessDeniedException();
        }
    }

    private async Task<Dictionary<Guid, ItemInstance>> ReadStockInstancesAsync(
        int campaignId,
        IReadOnlyCollection<ShopOffer> offers,
        CancellationToken cancellationToken )
    {
        Guid[] instanceKeys = offers
            .Where( offer => offer.ItemInstanceKey.HasValue )
            .Select( offer => offer.ItemInstanceKey!.Value )
            .Distinct()
            .ToArray();
        if ( instanceKeys.Length == 0 )
        {
            return [];
        }

        ItemInstance[] instances = await _inventoryDbContext.ItemInstances
            .AsNoTracking()
            .Where( instance =>
                instance.CampaignId == campaignId &&
                instanceKeys.Contains( instance.InstanceKey ) )
            .ToArrayAsync( cancellationToken );
        if ( instances.Length != instanceKeys.Length )
        {
            throw new InvalidOperationException(
                "Commerce offer references a missing or cross-campaign item instance." );
        }

        return instances.ToDictionary( instance => instance.InstanceKey );
    }

    private static int ResolveConfigurationId(
        ShopOffer offer,
        IReadOnlyDictionary<Guid, ItemInstance> stockInstances )
    {
        if ( offer.ItemConfigurationId is int configurationId )
        {
            return configurationId;
        }

        if ( offer.ItemInstanceKey is Guid instanceKey &&
             stockInstances.TryGetValue( instanceKey, out ItemInstance? instance ) )
        {
            return instance.ItemConfigurationId;
        }

        throw new InvalidOperationException(
            "Commerce offer does not resolve to an item configuration." );
    }

    private static SettlementDto ToDto( Settlement settlement ) =>
        new SettlementDto(
            settlement.Id,
            settlement.CampaignId,
            settlement.Name,
            settlement.Level,
            settlement.Region,
            settlement.Traits,
            settlement.Shops
                .OrderBy( shop => shop.Name )
                .ThenBy( shop => shop.Id )
                .Select( shop => new ShopDto(
                    shop.Id,
                    shop.CampaignId,
                    shop.SettlementId,
                    shop.Name,
                    shop.Specialization,
                    shop.ShopLevel,
                    shop.CatalogPricePercent,
                    shop.BuybackPricePercent,
                    shop.PricingPolicyVersion ) )
                .ToArray() );
}

public sealed class CommerceReadAccessDeniedException : Exception
{
    public CommerceReadAccessDeniedException()
        : base( "Commerce read access is denied." )
    {
    }
}

public sealed class CommerceReadNotFoundException : Exception
{
    public CommerceReadNotFoundException()
        : base( "Commerce read resource was not found." )
    {
    }
}
