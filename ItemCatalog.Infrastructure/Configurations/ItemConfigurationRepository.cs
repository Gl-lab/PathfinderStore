using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Application.Configurations;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Shared.Infrasturture.Repositories;

namespace Pathfinder.ItemCatalog.Infrastructure.Configurations;

public sealed class ItemConfigurationRepository
    : Repository<ItemConfiguration>, IItemConfigurationRepository
{
    private readonly ItemCatalogDbContext _dbContext;

    public ItemConfigurationRepository( ItemCatalogDbContext dbContext )
        : base( dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<ItemConfiguration?> GetByConfigurationKeyAsync(
        string configurationKey,
        CancellationToken cancellationToken )
    {
        return await _dbContext.ItemConfigurations
            .Include( configuration => configuration.PermanentUpgrades )
            .SingleOrDefaultAsync(
                configuration => configuration.ConfigurationKey == configurationKey,
                cancellationToken );
    }
}