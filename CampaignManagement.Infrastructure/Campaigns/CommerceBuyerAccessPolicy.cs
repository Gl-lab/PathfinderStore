using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Commerce.Application.Transactions;

namespace Pathfinder.CampaignManagement.Infrastructure.Campaigns;

public sealed class CommerceBuyerAccessPolicy : ICommerceBuyerAccessPolicy
{
    private readonly CampaignManagementDbContext _dbContext;

    public CommerceBuyerAccessPolicy( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ControlsCharacterAsync(
        int campaignId,
        int actingUserId,
        int characterId,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _dbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Memberships )
            .Include( item => item.Parties )
                .ThenInclude( party => party.Characters )
            .SingleOrDefaultAsync(
                item =>
                    ( item.Id == campaignId ) &&
                    ( item.Status == CampaignStatus.Active ),
                cancellationToken );
        if ( campaign is null ||
             !campaign.HasActiveRole( actingUserId, CampaignMembershipRole.Player ) )
        {
            return false;
        }

        return campaign.Parties.Any( party =>
            ( party.Status == CampaignPartyStatus.Active ) &&
            party.Characters.Any( character =>
                ( character.CharacterId == characterId ) &&
                ( character.ControlledByUserId == actingUserId ) ) );
    }
}
