using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Inventory.Application.Transfers;

namespace Pathfinder.CampaignManagement.Infrastructure.Campaigns;

public sealed class PartyTransferAccessPolicy : IPartyTransferAccessPolicy
{
    private readonly CampaignManagementDbContext _dbContext;

    public PartyTransferAccessPolicy( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<PartyTransferAccess> GetAccessAsync(
        int campaignId,
        int actingUserId,
        int sourceCharacterId,
        int destinationCharacterId,
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
        if ( campaign is null )
        {
            return PartyTransferAccess.Denied;
        }

        CampaignParty? party = campaign.Parties.SingleOrDefault( item =>
            ( item.Status == CampaignPartyStatus.Active ) &&
            item.Characters.Any( character => character.CharacterId == sourceCharacterId ) &&
            item.Characters.Any( character => character.CharacterId == destinationCharacterId ) );
        if ( party is null )
        {
            return PartyTransferAccess.Denied;
        }

        CampaignPartyCharacter sourceCharacter = party.Characters.Single( character =>
            character.CharacterId == sourceCharacterId );
        CampaignPartyCharacter destinationCharacter = party.Characters.Single( character =>
            character.CharacterId == destinationCharacterId );
        bool participantsAreActive =
            campaign.HasActiveRole(
                sourceCharacter.ControlledByUserId,
                CampaignMembershipRole.Player ) &&
            campaign.HasActiveRole(
                destinationCharacter.ControlledByUserId,
                CampaignMembershipRole.Player );
        if ( !participantsAreActive )
        {
            return PartyTransferAccess.Denied;
        }

        bool isPlayer = campaign.HasActiveRole( actingUserId, CampaignMembershipRole.Player );
        bool controlsSource = isPlayer &&
            ( sourceCharacter.ControlledByUserId == actingUserId );
        bool controlsDestination = isPlayer &&
            ( destinationCharacter.ControlledByUserId == actingUserId );
        return new PartyTransferAccess( true, party.Id, controlsSource, controlsDestination );
    }
}
