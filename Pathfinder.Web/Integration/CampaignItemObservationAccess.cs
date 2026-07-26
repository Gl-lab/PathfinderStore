using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class CampaignItemObservationAccess : IItemObservationAccess
{
    private readonly CampaignManagementDbContext _dbContext;

    public CampaignItemObservationAccess( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<ItemObservationAccess> GetAccessAsync(
        int campaignId,
        int observerUserId,
        CancellationToken cancellationToken )
    {
        CampaignMembershipRole[] roles = await _dbContext.CampaignMemberships
            .Where( membership =>
                membership.CampaignId == campaignId &&
                membership.UserId == observerUserId &&
                membership.Status == CampaignMembershipStatus.Active )
            .Select( membership => membership.Role )
            .ToArrayAsync( cancellationToken );
        bool isActiveCampaign = await _dbContext.Campaigns.AnyAsync(
            campaign =>
                campaign.Id == campaignId &&
                campaign.Status == CampaignStatus.Active,
            cancellationToken );
        return new ItemObservationAccess(
            isActiveCampaign && roles.Length > 0,
            isActiveCampaign && roles.Contains( CampaignMembershipRole.GameMaster ) );
    }
}