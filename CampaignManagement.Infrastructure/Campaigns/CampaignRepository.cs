using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Shared.Infrasturture.Repositories;

namespace Pathfinder.CampaignManagement.Infrastructure.Campaigns;

public sealed class CampaignRepository : Repository<Campaign>, ICampaignRepository
{
    private readonly CampaignManagementDbContext _dbContext;

    public CampaignRepository( CampaignManagementDbContext dbContext )
        : base( dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Campaign>> GetForUserAsync(
        int userId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Campaigns
            .AsNoTracking()
            .Include( campaign => campaign.Memberships )
            .Include( campaign => campaign.Invitations )
            .Where( campaign => campaign.Memberships.Any( membership =>
                ( membership.UserId == userId ) &&
                ( membership.Status == CampaignMembershipStatus.Active ) ) )
            .OrderByDescending( campaign => campaign.CreatedAtUtc )
            .ToArrayAsync( cancellationToken );
    }

    public async Task<Campaign?> GetByIdForUserAsync(
        int campaignId,
        int userId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Campaigns
            .Include( campaign => campaign.Memberships )
            .Include( campaign => campaign.Invitations )
            .SingleOrDefaultAsync(
                campaign =>
                    ( campaign.Id == campaignId ) &&
                    campaign.Memberships.Any( membership =>
                        ( membership.UserId == userId ) &&
                        ( membership.Status == CampaignMembershipStatus.Active ) ),
                cancellationToken );
    }

    public async Task<Campaign?> GetByInvitationIdForUserAsync(
        int invitationId,
        int userId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Campaigns
            .Include( campaign => campaign.Memberships )
            .Include( campaign => campaign.Invitations )
            .SingleOrDefaultAsync(
                campaign => campaign.Invitations.Any( invitation =>
                    ( invitation.Id == invitationId ) &&
                    ( invitation.InvitedUserId == userId ) &&
                    ( invitation.Status == CampaignInvitationStatus.Pending ) ),
                cancellationToken );
    }

    public async Task<IReadOnlyCollection<Campaign>> GetPendingInvitationsForUserAsync(
        int userId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Campaigns
            .AsNoTracking()
            .Include( campaign => campaign.Invitations )
            .Where( campaign => campaign.Invitations.Any( invitation =>
                ( invitation.InvitedUserId == userId ) &&
                ( invitation.Status == CampaignInvitationStatus.Pending ) ) )
            .ToArrayAsync( cancellationToken );
    }
}
