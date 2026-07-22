using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.ItemCatalog.Application.Items;

public interface IItemDefinitionRepository : IRepository<ItemDefinition>
{
    Task<ItemDefinition?> GetByIdWithRevisionsAsync(
        int itemDefinitionId,
        CancellationToken cancellationToken );
    Task<ItemDefinition?> GetByKeyWithRevisionsAsync(
        string key,
        CancellationToken cancellationToken );
}