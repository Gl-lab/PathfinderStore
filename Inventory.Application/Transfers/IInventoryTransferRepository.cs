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
    Task<PartyExchange?> GetExchangeAsync( Guid exchangeKey, CancellationToken cancellationToken );
    void AddGift( PartyGift gift );
    void AddExchange( PartyExchange exchange );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
