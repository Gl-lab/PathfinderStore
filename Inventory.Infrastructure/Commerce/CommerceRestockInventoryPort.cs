using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;

namespace Pathfinder.Inventory.Infrastructure.Commerce;

public sealed class CommerceRestockInventoryPort : ICommerceRestockInventoryPort
{
    private readonly InventoryDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public CommerceRestockInventoryPort(
        InventoryDbContext dbContext,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task EnsureShopContainerAsync(
        int campaignId,
        int shopId,
        CancellationToken cancellationToken )
    {
        bool exists = await _dbContext.Containers.AnyAsync(
            container =>
                container.CampaignId == campaignId &&
                container.OwnerKind == InventoryContainerOwnerKind.Shop &&
                container.OwnerId == shopId,
            cancellationToken );
        if ( exists )
        {
            return;
        }

        _dbContext.Containers.Add( InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaignId,
            InventoryContainerOwnerKind.Shop,
            shopId,
            _timeProvider.GetUtcNow() ) );
        await _dbContext.SaveChangesAsync( cancellationToken );
    }

    public async Task<Guid> EnsureUniqueShopStockAsync(
        int campaignId,
        int shopId,
        int itemConfigurationId,
        Guid itemInstanceKey,
        DateTimeOffset createdAtUtc,
        CancellationToken cancellationToken )
    {
        ItemInstance? existing = await _dbContext.ItemInstances
            .SingleOrDefaultAsync(
                item => item.InstanceKey == itemInstanceKey,
                cancellationToken );
        InventoryContainer container = await _dbContext.Containers
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.CampaignId == campaignId &&
                    candidate.OwnerKind == InventoryContainerOwnerKind.Shop &&
                    candidate.OwnerId == shopId,
                cancellationToken ) ?? throw new InventoryException(
            "Shop inventory container was not found." );
        if ( existing is not null )
        {
            if ( existing.CampaignId != campaignId ||
                 existing.ItemConfigurationId != itemConfigurationId ||
                 existing.CurrentContainerKey != container.ContainerKey ||
                 existing.IsStackable ||
                 existing.Quantity != 1 )
            {
                throw new InventoryException(
                    "Restock item instance key was already used for different stock." );
            }

            return existing.InstanceKey;
        }

        ItemInstance item = ItemInstance.Create(
            itemInstanceKey,
            campaignId,
            itemConfigurationId,
            container,
            null,
            createdAtUtc );
        _dbContext.ItemInstances.Add( item );
        await _dbContext.SaveChangesAsync( cancellationToken );
        return item.InstanceKey;
    }

    public async Task DiscardUniqueShopStockAsync(
        int campaignId,
        int shopId,
        int itemConfigurationId,
        Guid itemInstanceKey,
        CancellationToken cancellationToken )
    {
        ItemInstance? item = await _dbContext.ItemInstances
            .SingleOrDefaultAsync(
                candidate => candidate.InstanceKey == itemInstanceKey,
                cancellationToken );
        if ( item is null )
        {
            return;
        }

        InventoryContainer container = await _dbContext.Containers
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.CampaignId == campaignId &&
                    candidate.OwnerKind == InventoryContainerOwnerKind.Shop &&
                    candidate.OwnerId == shopId,
                cancellationToken ) ?? throw new InventoryException(
            "Shop inventory container was not found." );
        if ( item.CampaignId != campaignId ||
             item.ItemConfigurationId != itemConfigurationId ||
             item.CurrentContainerKey != container.ContainerKey ||
             item.Version != 0 ||
             item.ReservationKey is not null )
        {
            throw new InventoryException(
                "Prepared restock item can no longer be safely discarded." );
        }

        _dbContext.ItemInstances.Remove( item );
        await _dbContext.SaveChangesAsync( cancellationToken );
    }
}
