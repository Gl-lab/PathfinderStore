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
        int[] controlledCharacterIds = await _dbContext.CampaignPartyCharacters
            .Join(
                _dbContext.CampaignParties.Where( party =>
                    party.CampaignId == campaignId &&
                    party.Status == CampaignPartyStatus.Active ),
                assignment => assignment.CampaignPartyId,
                party => party.Id,
                ( assignment, party ) => assignment )
            .Where( assignment => assignment.ControlledByUserId == observerUserId )
            .Select( assignment => assignment.CharacterId )
            .ToArrayAsync( cancellationToken );
        int[] partyIds = await _dbContext.CampaignPartyCharacters
            .Join(
                _dbContext.CampaignParties.Where( party =>
                    party.CampaignId == campaignId &&
                    party.Status == CampaignPartyStatus.Active ),
                assignment => assignment.CampaignPartyId,
                party => party.Id,
                ( assignment, party ) => new
                {
                    Assignment = assignment,
                    PartyId = party.Id,
                } )
            .Where( item => item.Assignment.ControlledByUserId == observerUserId )
            .Select( item => item.PartyId )
            .Distinct()
            .ToArrayAsync( cancellationToken );
        return new ItemObservationAccess(
            isActiveCampaign && roles.Length > 0,
            isActiveCampaign && roles.Contains( CampaignMembershipRole.GameMaster ),
            controlledCharacterIds,
            partyIds );
    }
}