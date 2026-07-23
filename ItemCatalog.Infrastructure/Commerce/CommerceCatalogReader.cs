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
            ( configuration.Id == itemConfigurationId ) &&
            _dbContext.ItemRevisions.Any( revision =>
                ( revision.Id == configuration.ItemRevisionId ) &&
                ( revision.Status == ItemRevisionStatus.Published ) &&
                _dbContext.ItemDefinitions.Any( definition =>
                    ( definition.Id == revision.ItemDefinitionId ) &&
                    ( ( definition.Scope == ItemCatalogScope.Global ) ||
                      ( definition.CampaignId == campaignId ) ) ) ),
        cancellationToken );
}
