namespace Pathfinder.Commerce.Application.Shops;

public interface ICommerceCampaignAccessPolicy
{
    Task<bool> IsGameMasterAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken );
}
