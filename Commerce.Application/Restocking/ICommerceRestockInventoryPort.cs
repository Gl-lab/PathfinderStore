namespace Pathfinder.Commerce.Application.Restocking;

public interface ICommerceRestockInventoryPort
{
    Task EnsureShopContainerAsync(
        int campaignId,
        int shopId,
        CancellationToken cancellationToken );
    Task<Guid> EnsureUniqueShopStockAsync(
        int campaignId,
        int shopId,
        int itemConfigurationId,
        Guid itemInstanceKey,
        DateTimeOffset createdAtUtc,
        CancellationToken cancellationToken );
    Task DiscardUniqueShopStockAsync(
        int campaignId,
        int shopId,
        int itemConfigurationId,
        Guid itemInstanceKey,
        CancellationToken cancellationToken );
}
