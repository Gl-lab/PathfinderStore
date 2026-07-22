using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Inventory.Application.Storage;

namespace Pathfinder.CampaignManagement.Infrastructure.Campaigns;

public sealed class PartyStorageAccessPolicy : IPartyStorageAccessPolicy
{
    private readonly CampaignManagementDbContext _dbContext;

    public PartyStorageAccessPolicy( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<PartyStorageAccess> GetAccessAsync(
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
            .Include( item => item.Parties )
                .ThenInclude( party => party.Storage )
            .SingleOrDefaultAsync(
                item =>
                    ( item.Id == campaignId ) &&
                    ( item.Status == CampaignStatus.Active ),
                cancellationToken );
        CampaignParty? party = campaign?.Parties.SingleOrDefault( item =>
            ( item.Status == CampaignPartyStatus.Active ) &&
            item.Characters.Any( character => character.CharacterId == characterId ) );
        if ( ( campaign is null ) || ( party is null ) )
        {
            return PartyStorageAccess.Denied;
        }

        bool isGameMaster = campaign.HasActiveRole(
            actingUserId,
            CampaignMembershipRole.GameMaster );
        bool controlsCharacter = campaign.HasActiveRole(
            actingUserId,
            CampaignMembershipRole.Player ) && party.Characters.Any( character =>
            ( character.CharacterId == characterId ) &&
            ( character.ControlledByUserId == actingUserId ) );
        PartyStorageWithdrawalPolicy policy = party.Storage.AccessPolicy switch
        {
            CampaignPartyStorageAccessPolicy.FreeForMembers =>
                PartyStorageWithdrawalPolicy.FreeForMembers,
            CampaignPartyStorageAccessPolicy.GameMasterOnly =>
                PartyStorageWithdrawalPolicy.GameMasterOnly,
            _ => PartyStorageWithdrawalPolicy.Unconfigured,
        };
        return new PartyStorageAccess(
            true,
            party.Id,
            controlsCharacter,
            isGameMaster,
            policy );
    }
}
