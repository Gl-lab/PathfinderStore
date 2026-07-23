namespace Pathfinder.Commerce.Application.Offers;

public sealed record CommerceStockItem(
    Guid ItemInstanceKey,
    int CampaignId,
    int OwnerShopId,
    int Quantity,
    bool IsAvailable );

public interface ICommerceInventoryReader
{
    Task EnsureShopContainerAsync(
        int campaignId,
        int shopId,
        CancellationToken cancellationToken );
    Task<CommerceStockItem?> GetShopStockAsync(
        Guid itemInstanceKey,
        CancellationToken cancellationToken );
}
