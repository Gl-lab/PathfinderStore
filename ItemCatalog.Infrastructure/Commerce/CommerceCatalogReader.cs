using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.ItemCatalog.Infrastructure.Commerce;

public sealed class CommerceCatalogReader : ICommerceCatalogReader
{
    private readonly ItemCatalogDbContext _dbContext;

    public CommerceCatalogReader( ItemCatalogDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<bool> IsPublishedConfigurationAsync(
        int itemConfigurationId,
        int campaignId,
        CancellationToken cancellationToken ) => _dbContext.ItemConfigurations.AnyAsync(
        configuration =>
            configuration.Id == itemConfigurationId &&
            _dbContext.ItemRevisions.Any( revision =>
                revision.Id == configuration.ItemRevisionId &&
                revision.Status == ItemRevisionStatus.Published &&
                _dbContext.ItemDefinitions.Any( definition =>
                    definition.Id == revision.ItemDefinitionId &&
                    ( definition.Scope == ItemCatalogScope.Global ||
                      definition.CampaignId == campaignId ) ) ),
        cancellationToken );

    public async Task<long?> GetBasePriceCopperAsync(
        int itemConfigurationId,
        int campaignId,
        CancellationToken cancellationToken )
    {
        int? price = await (
            from configuration in _dbContext.ItemConfigurations
            join revision in _dbContext.ItemRevisions
                on configuration.ItemRevisionId equals revision.Id
            join definition in _dbContext.ItemDefinitions
                on revision.ItemDefinitionId equals definition.Id
            where
                configuration.Id == itemConfigurationId &&
                revision.Status == ItemRevisionStatus.Published &&
                ( definition.Scope == ItemCatalogScope.Global ||
                  definition.CampaignId == campaignId )
            select ( int? )revision.PriceInCopperPieces )
            .SingleOrDefaultAsync( cancellationToken );
        return price;
    }

    public async Task<IReadOnlyCollection<CommerceCatalogCandidate>> GetRestockCandidatesAsync(
        int campaignId,
        CancellationToken cancellationToken )
    {
        List<CommerceCatalogCandidate> candidates = await (
            from configuration in _dbContext.ItemConfigurations
            join revision in _dbContext.ItemRevisions
                on configuration.ItemRevisionId equals revision.Id
            join definition in _dbContext.ItemDefinitions
                on revision.ItemDefinitionId equals definition.Id
            where
                revision.Status == ItemRevisionStatus.Published &&
                ( definition.Scope == ItemCatalogScope.Global ||
                  definition.CampaignId == campaignId ) &&
                ( configuration.CampaignId == null ||
                  configuration.CampaignId == campaignId )
            orderby configuration.Id
            select new CommerceCatalogCandidate(
                configuration.Id,
                revision.Level,
                revision.PriceInCopperPieces,
                ( int )revision.PrimaryCategory,
                ( int )revision.Rarity,
                definition.Scope == ItemCatalogScope.Campaign ) )
            .ToListAsync( cancellationToken );
        return candidates;
    }
}
