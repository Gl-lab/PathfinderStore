using Pathfinder.Commerce.Domain.Offers;

namespace Pathfinder.Commerce.Application.Transactions;

public sealed record CommerceSellableItem(
    Guid ItemInstanceKey,
    int CampaignId,
    int OwnerCharacterId,
    int ItemConfigurationId,
    int Quantity,
    int Version,
    bool CanTransfer );

public interface ICommerceInventoryTradePort
{
    Task<Guid> CompletePurchaseAsync(
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
        CancellationToken cancellationToken );
    Task<CommerceSellableItem?> GetSellableItemAsync(
        int campaignId,
        int characterId,
        Guid itemInstanceKey,
        Guid operationId,
        int actingUserId,
        CancellationToken cancellationToken );
    Task MoveSaleToShopAsync(
        int campaignId,
        int shopId,
        CommerceSellableItem item,
        Guid operationId,
        int actingUserId,
        DateTimeOffset occurredAtUtc,
        CancellationToken cancellationToken );
}
