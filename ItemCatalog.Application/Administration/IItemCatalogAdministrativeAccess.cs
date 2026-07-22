namespace Pathfinder.ItemCatalog.Application.Administration;

public interface IItemCatalogAdministrativeAccess
{
    Task<bool> CanManageGlobalCatalogAsync(
        string userName,
        CancellationToken cancellationToken );
    Task<bool> CanManageCampaignCatalogAsync(
        int userId,
        int campaignId,
        CancellationToken cancellationToken );
}