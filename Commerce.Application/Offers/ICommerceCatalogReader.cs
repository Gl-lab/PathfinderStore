namespace Pathfinder.Commerce.Application.Offers;

public interface ICommerceCatalogReader
{
    Task<bool> IsPublishedConfigurationAsync(
        int itemConfigurationId,
        int campaignId,
        CancellationToken cancellationToken );
}
