using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;

namespace Pathfinder.Inventory.Infrastructure.Commerce;

public sealed class CommerceInventoryReader : ICommerceInventoryReader
{
    private readonly InventoryDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public CommerceInventoryReader( InventoryDbContext dbContext, TimeProvider timeProvider )
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
                ( container.CampaignId == campaignId ) &&
                ( container.OwnerKind == InventoryContainerOwnerKind.Shop ) &&
                ( container.OwnerId == shopId ),
            cancellationToken );
        if ( exists )
        {
            return;
        }

        InventoryContainer container = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaignId,
            InventoryContainerOwnerKind.Shop,
            shopId,
            _timeProvider.GetUtcNow() );
        _dbContext.Containers.Add( container );
        await _dbContext.SaveChangesAsync( cancellationToken );
    }

    public async Task<CommerceStockItem?> GetShopStockAsync(
        Guid itemInstanceKey,
        CancellationToken cancellationToken )
    {
        ItemInstance? item = await _dbContext.ItemInstances
            .AsNoTracking()
            .SingleOrDefaultAsync(
                instance => instance.InstanceKey == itemInstanceKey,
                cancellationToken );
        if ( item is null )
        {
            return null;
        }

        InventoryContainer? container = await _dbContext.Containers
            .AsNoTracking()
            .SingleOrDefaultAsync(
                candidate =>
                    ( candidate.CampaignId == item.CampaignId ) &&
                    ( candidate.ContainerKey == item.CurrentContainerKey ) &&
                    ( candidate.OwnerKind == InventoryContainerOwnerKind.Shop ),
                cancellationToken );
        return container is null
            ? null
            : new CommerceStockItem(
                item.InstanceKey,
                item.CampaignId,
                container.OwnerId,
                item.Quantity,
                !item.IsDepleted && ( item.ReservationKey is null ) );
    }
}
