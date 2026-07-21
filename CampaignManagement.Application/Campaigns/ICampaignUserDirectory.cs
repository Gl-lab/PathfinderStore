namespace Pathfinder.CampaignManagement.Application.Campaigns;

public interface ICampaignUserDirectory
{
    Task<int?> FindUserIdByNameAsync( string userName, CancellationToken cancellationToken );
}
