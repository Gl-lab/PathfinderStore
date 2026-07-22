using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Inventory.Application.Administration;

namespace Pathfinder.CampaignManagement.Infrastructure.Campaigns;

public sealed class InventoryGameMasterAccessPolicy : IInventoryGameMasterAccessPolicy
{
    private readonly CampaignManagementDbContext _dbContext;

    public InventoryGameMasterAccessPolicy( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsGameMasterAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _dbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Memberships )
            .SingleOrDefaultAsync(
                item =>
                    ( item.Id == campaignId ) &&
                    ( item.Status == CampaignStatus.Active ),
                cancellationToken );
        return campaign?.HasActiveRole(
            actingUserId,
            CampaignMembershipRole.GameMaster ) ?? false;
    }
}
