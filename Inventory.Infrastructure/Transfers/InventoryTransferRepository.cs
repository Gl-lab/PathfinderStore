using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Infrastructure.Data;

namespace Pathfinder.Inventory.Infrastructure.Transfers;

public sealed class InventoryTransferRepository : IInventoryTransferRepository
{
    private readonly InventoryDbContext _dbContext;

    public InventoryTransferRepository( InventoryDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<InventoryContainer?> GetCharacterContainerAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Containers.SingleOrDefaultAsync(
            container =>
                ( container.CampaignId == campaignId ) &&
                ( container.OwnerKind == InventoryContainerOwnerKind.Character ) &&
                ( container.OwnerId == characterId ),
            cancellationToken );
    }

    public async Task<InventoryContainer?> GetContainerAsync(
        int campaignId,
        InventoryContainerOwnerKind ownerKind,
        int ownerId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Containers.SingleOrDefaultAsync(
            container =>
                ( container.CampaignId == campaignId ) &&
                ( container.OwnerKind == ownerKind ) &&
                ( container.OwnerId == ownerId ),
            cancellationToken );
    }

    public async Task<ItemInstance?> GetItemAsync(
        Guid itemInstanceKey,
        CancellationToken cancellationToken )
    {
        return await _dbContext.ItemInstances
            .Include( item => item.Movements )
            .Include( item => item.Operations )
            .SingleOrDefaultAsync(
                item => item.InstanceKey == itemInstanceKey,
                cancellationToken );
    }

    public async Task<PartyGift?> GetGiftAsync(
        Guid giftKey,
        CancellationToken cancellationToken )
    {
        return await _dbContext.PartyGifts.SingleOrDefaultAsync(
            gift => gift.GiftKey == giftKey,
            cancellationToken );
    }

    public async Task<PartyExchange?> GetExchangeAsync(
        Guid exchangeKey,
        CancellationToken cancellationToken )
    {
        return await _dbContext.PartyExchanges
            .Include( exchange => exchange.Lines )
            .SingleOrDefaultAsync(
                exchange => exchange.ExchangeKey == exchangeKey,
                cancellationToken );
    }

    public void AddGift( PartyGift gift )
    {
        _dbContext.PartyGifts.Add( gift );
    }

    public void AddExchange( PartyExchange exchange )
    {
        _dbContext.PartyExchanges.Add( exchange );
    }

    public void AddContainer( InventoryContainer container )
    {
        _dbContext.Containers.Add( container );
    }

    public async Task SaveChangesAsync( CancellationToken cancellationToken )
    {
        await _dbContext.SaveChangesAsync( cancellationToken );
    }
}
