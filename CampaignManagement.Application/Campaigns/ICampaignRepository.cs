using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public interface ICampaignRepository : IRepository<Campaign>
{
    Task<IReadOnlyCollection<Campaign>> GetForUserAsync( int userId, CancellationToken cancellationToken );
    Task<Campaign?> GetByIdForUserAsync( int campaignId, int userId, CancellationToken cancellationToken );
    Task<Campaign?> GetByInvitationIdForUserAsync(
        int invitationId,
        int userId,
        CancellationToken cancellationToken );
    Task<IReadOnlyCollection<Campaign>> GetPendingInvitationsForUserAsync(
        int userId,
        CancellationToken cancellationToken );
}
