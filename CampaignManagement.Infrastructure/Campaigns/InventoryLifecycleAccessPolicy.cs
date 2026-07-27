using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Inventory.Application.Lifecycle;
using Pathfinder.Inventory.Domain.Containers;

namespace Pathfinder.CampaignManagement.Infrastructure.Campaigns;

public sealed class InventoryLifecycleAccessPolicy : IInventoryLifecycleAccessPolicy
{
    private readonly CampaignManagementDbContext _dbContext;

    public InventoryLifecycleAccessPolicy( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CanMutateAsync(
        int campaignId,
        int actingUserId,
        InventoryContainerOwnerKind ownerKind,
        int ownerId,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _dbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Memberships )
            .Include( item => item.Parties )
                .ThenInclude( party => party.Characters )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == campaignId &&
                    item.Status == CampaignStatus.Active,
                cancellationToken );
        if ( campaign is null )
        {
            return false;
        }

        if ( campaign.HasActiveRole(
            actingUserId,
            CampaignMembershipRole.GameMaster ) )
        {
            return true;
        }

        return ownerKind == InventoryContainerOwnerKind.Character &&
               campaign.HasActiveRole(
                   actingUserId,
                   CampaignMembershipRole.Player ) &&
               campaign.Parties.Any( party =>
                   party.Status == CampaignPartyStatus.Active &&
                   party.Characters.Any( character =>
                       character.CharacterId == ownerId &&
                       character.ControlledByUserId == actingUserId ) );
    }
}
