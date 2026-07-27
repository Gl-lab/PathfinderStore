namespace Pathfinder.Commerce.Application.Offers;

public sealed record CommerceCatalogCandidate(
    int ItemConfigurationId,
    int Level,
    long BasePriceCopper,
    int PrimaryCategory,
    bool IsCampaignScoped );

public interface ICommerceCatalogReader
{
    Task<bool> IsPublishedConfigurationAsync(
        int itemConfigurationId,
        int campaignId,
        CancellationToken cancellationToken );
    Task<long?> GetBasePriceCopperAsync(
        int itemConfigurationId,
        int campaignId,
        CancellationToken cancellationToken ) => Task.FromResult<long?>( null );
    Task<IReadOnlyCollection<CommerceCatalogCandidate>> GetRestockCandidatesAsync(
        int campaignId,
        CancellationToken cancellationToken ) =>
        Task.FromResult<IReadOnlyCollection<CommerceCatalogCandidate>>( [] );
}
