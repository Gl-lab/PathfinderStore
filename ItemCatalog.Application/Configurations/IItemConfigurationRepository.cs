using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.ItemCatalog.Application.Configurations;

public interface IItemConfigurationRepository : IRepository<ItemConfiguration>
{
    Task<ItemConfiguration?> GetByConfigurationKeyAsync(
        string configurationKey,
        CancellationToken cancellationToken );
}