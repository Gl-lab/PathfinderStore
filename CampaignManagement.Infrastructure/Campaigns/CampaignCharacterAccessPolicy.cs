using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Application.Access;

namespace Pathfinder.CampaignManagement.Infrastructure.Campaigns;

public sealed class CampaignCharacterAccessPolicy : ICharacterCampaignAccessPolicy
{
    private readonly CampaignManagementDbContext _dbContext;

    public CampaignCharacterAccessPolicy( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<CharacterCampaignAccess> GetAccessAsync(
        int campaignId,
        int userId,
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
                    item.Id == campaignId &&
                    item.Status == CampaignStatus.Active,
                cancellationToken );
        if ( campaign is null )
        {
            return CharacterCampaignAccess.Denied;
        }

        bool isGameMaster = campaign.HasActiveRole( userId, CampaignMembershipRole.GameMaster );
        bool belongsToActiveParty = campaign.Parties.Any( party =>
            party.Status == CampaignPartyStatus.Active &&
            party.Characters.Any( character => character.CharacterId == characterId ) );
        bool controlsCharacter = campaign.HasActiveRole( userId, CampaignMembershipRole.Player ) &&
            campaign.Parties.Any( party =>
                party.Status == CampaignPartyStatus.Active &&
                party.Characters.Any( character =>
                    character.CharacterId == characterId &&
                    character.ControlledByUserId == userId ) );
        return new CharacterCampaignAccess(
            ( isGameMaster && belongsToActiveParty ) || controlsCharacter,
            controlsCharacter );
    }
}
