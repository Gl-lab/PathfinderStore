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
        return await _dbContext.ItemDefinitions
            .Include( definition => definition.Revisions )
            .SingleOrDefaultAsync(
                definition => definition.Id == itemDefinitionId,
                cancellationToken );
    }

    public async Task<ItemDefinition?> GetByKeyWithRevisionsAsync(
        string key,
        CancellationToken cancellationToken )
    {
        return await _dbContext.ItemDefinitions
            .Include( definition => definition.Revisions )
            .SingleOrDefaultAsync(
                definition => definition.Key == key,
                cancellationToken );
    }
}