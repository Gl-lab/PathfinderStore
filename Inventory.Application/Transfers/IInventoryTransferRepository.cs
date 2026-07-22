using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Domain.Audit;

namespace Pathfinder.Inventory.Application.Transfers;

public interface IInventoryTransferRepository
{
    Task<InventoryContainer?> GetCharacterContainerAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken );
    Task<InventoryContainer?> GetContainerAsync(
        int campaignId,
        InventoryContainerOwnerKind ownerKind,
        int ownerId,
        CancellationToken cancellationToken );
    Task<InventoryContainer?> GetContainerByKeyAsync(
        int campaignId,
        Guid containerKey,
        CancellationToken cancellationToken );
    Task<ItemInstance?> GetItemAsync( Guid itemInstanceKey, CancellationToken cancellationToken );
    Task<PartyGift?> GetGiftAsync( Guid giftKey, CancellationToken cancellationToken );
    Task<PartyExchange?> GetExchangeAsync( Guid exchangeKey, CancellationToken cancellationToken );
    Task<InventoryAuditEntry?> GetAuditAsync(
        int campaignId,
        Guid operationId,
        InventoryAuditActionKind actionKind,
        CancellationToken cancellationToken );
    void AddGift( PartyGift gift );
    void AddExchange( PartyExchange exchange );
    void AddContainer( InventoryContainer container );
    void AddAudit( InventoryAuditEntry audit );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
