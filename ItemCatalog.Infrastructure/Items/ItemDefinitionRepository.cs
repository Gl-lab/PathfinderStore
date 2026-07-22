using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Application.Items;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Shared.Infrasturture.Repositories;

namespace Pathfinder.ItemCatalog.Infrastructure.Items;

public sealed class ItemDefinitionRepository : Repository<ItemDefinition>, IItemDefinitionRepository
{
    private readonly ItemCatalogDbContext _dbContext;

    public ItemDefinitionRepository( ItemCatalogDbContext dbContext )
        : base( dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<ItemDefinition?> GetByIdWithRevisionsAsync(
        int itemDefinitionId,
        CancellationToken cancellationToken )
    {
        return await WithRevisions()
            .SingleOrDefaultAsync(
                definition => definition.Id == itemDefinitionId,
                cancellationToken );
    }

    public async Task<ItemDefinition?> GetGlobalByKeyWithRevisionsAsync(
        string key,
        CancellationToken cancellationToken )
    {
        return await WithRevisions()
            .SingleOrDefaultAsync(
                definition =>
                    ( definition.Scope == ItemCatalogScope.Global ) &&
                    ( definition.Key == key ),
                cancellationToken );
    }

    public async Task<ItemDefinition?> GetCampaignByKeyWithRevisionsAsync(
        string key,
        int campaignId,
        CancellationToken cancellationToken )
    {
        return await WithRevisions()
            .SingleOrDefaultAsync(
                definition =>
                    ( definition.Scope == ItemCatalogScope.Campaign ) &&
                    ( definition.CampaignId == campaignId ) &&
                    ( definition.Key == key ),
                cancellationToken );
    }

    public async Task<IReadOnlyCollection<ItemDefinition>> GetVisibleWithRevisionsAsync(
        int campaignId,
        CancellationToken cancellationToken )
    {
        return await WithRevisions()
            .Where( definition =>
                ( definition.Scope == ItemCatalogScope.Global ) ||
                ( ( definition.Scope == ItemCatalogScope.Campaign ) &&
                  ( definition.CampaignId == campaignId ) ) )
            .OrderBy( definition => definition.Key )
            .ToArrayAsync( cancellationToken );
    }

    private IQueryable<ItemDefinition> WithRevisions()
    {
        return _dbContext.ItemDefinitions
            .AsSplitQuery()
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Attacks )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Armor )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Shield )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Equipment )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Consumption )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Charges )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Durability );
    }
}