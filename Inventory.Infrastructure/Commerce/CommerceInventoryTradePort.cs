using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Operations;
using Pathfinder.Inventory.Infrastructure.Data;

namespace Pathfinder.Inventory.Infrastructure.Commerce;

public sealed class CommerceInventoryTradePort : ICommerceInventoryTradePort
{
    private readonly InventoryDbContext _dbContext;
    private readonly IItemTransferRestrictionPolicy _restrictionPolicy;

    public CommerceInventoryTradePort(
        InventoryDbContext dbContext,
        IItemTransferRestrictionPolicy restrictionPolicy )
    {
        _dbContext = dbContext;
        _restrictionPolicy = restrictionPolicy;
    }

    public async Task<Guid> CompletePurchaseAsync(
        int campaignId,
        int shopId,
        int buyerCharacterId,
        ShopOfferKind offerKind,
        int? itemConfigurationId,
        Guid? itemInstanceKey,
        int quantity,
        Guid operationId,
        int actingUserId,
        DateTimeOffset occurredAtUtc,
        CancellationToken cancellationToken )
    {
        InventoryContainer destination = await GetContainerAsync(
            campaignId,
            InventoryContainerOwnerKind.Character,
            buyerCharacterId,
            cancellationToken );
        ItemInstance? replay = await _dbContext.ItemInstances
            .Include( item => item.Movements )
            .Include( item => item.Operations )
            .SingleOrDefaultAsync(
                item =>
                    ( item.CampaignId == campaignId ) &&
                    ( ( item.InstanceKey == operationId ) ||
                      ( item.InstanceKey == itemInstanceKey ) ) &&
                    ( item.CurrentContainerKey == destination.ContainerKey ),
                cancellationToken );
        if ( replay is not null )
        {
            return replay.InstanceKey;
        }

        if ( offerKind == ShopOfferKind.Catalog )
        {
            int configurationId = itemConfigurationId ?? throw new CommerceException(
                "Catalog offer does not reference a configuration." );
            ItemInstance created = quantity == 1
                ? ItemInstance.Create(
                    operationId,
                    campaignId,
                    configurationId,
                    destination,
                    null,
                    occurredAtUtc )
                : ItemInstance.CreateStack(
                    operationId,
                    campaignId,
                    configurationId,
                    quantity,
                    destination,
                    null,
                    occurredAtUtc );
            _dbContext.ItemInstances.Add( created );
            await _dbContext.SaveChangesAsync( cancellationToken );
            return created.InstanceKey;
        }

        InventoryContainer shopContainer = await GetContainerAsync(
            campaignId,
            InventoryContainerOwnerKind.Shop,
            shopId,
            cancellationToken );
        Guid stockKey = itemInstanceKey ?? throw new CommerceException(
            "Stock offer does not reference an item instance." );
        ItemInstance stock = await _dbContext.ItemInstances
            .Include( item => item.Movements )
            .Include( item => item.Operations )
            .SingleOrDefaultAsync(
                item =>
                    ( item.CampaignId == campaignId ) &&
                    ( item.InstanceKey == stockKey ),
                cancellationToken ) ?? throw new CommerceException( "Shop stock item was not found." );
        if ( stock.CurrentContainerKey != shopContainer.ContainerKey )
        {
            throw new CommerceException( "Shop stock item changed after reservation." );
        }

        ItemInstance purchased = stock;
        if ( quantity < stock.Quantity )
        {
            ItemSplitResult split = stock.Split(
                operationId,
                quantity,
                stock.Version,
                operationId,
                occurredAtUtc );
            purchased = split.NewInstance ?? throw new CommerceException(
                "Stock split replay could not resolve the purchased item." );
            _dbContext.ItemInstances.Add( purchased );
        }
        else if ( quantity != stock.Quantity )
        {
            throw new CommerceException( "Shop stock quantity changed after reservation." );
        }

        purchased.MoveTo(
            destination,
            "Shop purchase",
            purchased.Version,
            operationId,
            $"user:{actingUserId}",
            occurredAtUtc );
        await _dbContext.SaveChangesAsync( cancellationToken );
        return purchased.InstanceKey;
    }

    public async Task<CommerceSellableItem?> GetSellableItemAsync(
        int campaignId,
        int characterId,
        Guid itemInstanceKey,
        Guid operationId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        InventoryContainer? container = await _dbContext.Containers
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    ( item.CampaignId == campaignId ) &&
                    ( item.OwnerKind == InventoryContainerOwnerKind.Character ) &&
                    ( item.OwnerId == characterId ),
                cancellationToken );
        ItemInstance? item = await _dbContext.ItemInstances
            .AsNoTracking()
            .Include( candidate => candidate.Movements )
            .SingleOrDefaultAsync(
                candidate =>
                    ( candidate.CampaignId == campaignId ) &&
                    ( candidate.InstanceKey == itemInstanceKey ),
                cancellationToken );
        if ( ( container is null ) || ( item is null ) )
        {
            return null;
        }

        bool isReplay = item.Movements.Any( movement =>
            ( movement.OperationId == operationId ) &&
            ( movement.FromContainerKey == container.ContainerKey ) );
        bool isEquipped = false;
        if ( !isReplay )
        {
            if ( item.CurrentContainerKey != container.ContainerKey )
            {
                return null;
            }

            isEquipped = await _restrictionPolicy.IsEquippedAsync(
                characterId,
                itemInstanceKey,
                cancellationToken );
        }

        bool canTransfer = isReplay ||
            ( !item.IsDepleted &&
              ( item.ReservationKey is null ) &&
              !item.IsTransferRestricted &&
              !isEquipped );
        return new CommerceSellableItem(
            item.InstanceKey,
            item.CampaignId,
            characterId,
            item.ItemConfigurationId,
            item.Quantity,
            item.Version,
            canTransfer );
    }

    public async Task MoveSaleToShopAsync(
        int campaignId,
        int shopId,
        CommerceSellableItem item,
        Guid operationId,
        int actingUserId,
        DateTimeOffset occurredAtUtc,
        CancellationToken cancellationToken )
    {
        InventoryContainer destination = await GetContainerAsync(
            campaignId,
            InventoryContainerOwnerKind.Shop,
            shopId,
            cancellationToken );
        ItemInstance instance = await _dbContext.ItemInstances
            .Include( candidate => candidate.Movements )
            .Include( candidate => candidate.Operations )
            .SingleOrDefaultAsync(
                candidate => candidate.InstanceKey == item.ItemInstanceKey,
                cancellationToken ) ?? throw new CommerceException( "Sale item was not found." );
        if ( instance.CurrentContainerKey == destination.ContainerKey )
        {
            return;
        }

        instance.MoveTo(
            destination,
            "Shop sale",
            item.Version,
            operationId,
            $"user:{actingUserId}",
            occurredAtUtc );
        await _dbContext.SaveChangesAsync( cancellationToken );
    }

    private async Task<InventoryContainer> GetContainerAsync(
        int campaignId,
        InventoryContainerOwnerKind ownerKind,
        int ownerId,
        CancellationToken cancellationToken ) => await _dbContext.Containers.SingleOrDefaultAsync(
        container =>
            ( container.CampaignId == campaignId ) &&
            ( container.OwnerKind == ownerKind ) &&
            ( container.OwnerId == ownerId ),
        cancellationToken ) ?? throw new CommerceException( "Inventory container was not found." );
}
