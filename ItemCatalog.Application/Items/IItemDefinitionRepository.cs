using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.ItemCatalog.Application.Items;

public interface IItemDefinitionRepository : IRepository<ItemDefinition>
{
    Task<ItemDefinition?> GetByIdWithRevisionsAsync(
        int itemDefinitionId,
        CancellationToken cancellationToken );
    Task<ItemDefinition?> GetGlobalByKeyWithRevisionsAsync(
        string key,
        CancellationToken cancellationToken );
    Task<ItemDefinition?> GetCampaignByKeyWithRevisionsAsync(
        string key,
        int campaignId,
        CancellationToken cancellationToken );
    Task<IReadOnlyCollection<ItemDefinition>> GetVisibleWithRevisionsAsync(
        int campaignId,
        CancellationToken cancellationToken );
}