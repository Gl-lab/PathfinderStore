using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Application.Transfers;

public interface IInventoryTransferRepository
{
    Task<InventoryContainer?> GetCharacterContainerAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken );
    Task<ItemInstance?> GetItemAsync( Guid itemInstanceKey, CancellationToken cancellationToken );
    Task<PartyGift?> GetGiftAsync( Guid giftKey, CancellationToken cancellationToken );
    void AddGift( PartyGift gift );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
