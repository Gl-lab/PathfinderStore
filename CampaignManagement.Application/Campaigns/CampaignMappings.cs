using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

internal static class CampaignMappings
{
    public static CampaignDto ToDto( this Campaign campaign, int userId )
    {
        IReadOnlyCollection<CampaignMembershipRole> roles = campaign.Memberships
            .Where( membership =>
                ( membership.UserId == userId ) &&
                ( membership.Status == CampaignMembershipStatus.Active ) )
            .Select( membership => membership.Role )
            .Distinct()
            .ToArray();
        IReadOnlyCollection<CampaignMemberDto> members = campaign.Memberships
            .Where( membership => membership.Status == CampaignMembershipStatus.Active )
            .GroupBy( membership => membership.UserId )
            .Select( group => new CampaignMemberDto(
                group.Key,
                group.Select( membership => membership.Role )
                    .Distinct()
                    .ToArray() ) )
            .OrderBy( member => member.UserId )
            .ToArray();
        IReadOnlyCollection<CampaignPartyDto> parties = campaign.Parties
            .OrderBy( party => party.CreatedAtUtc )
            .Select( party => new CampaignPartyDto(
                party.Id,
                party.Name,
                party.Status,
                party.CreatedAtUtc,
                party.ArchivedAtUtc,
                party.Characters
                    .OrderBy( character => character.AssignedAtUtc )
                    .Select( character => new CampaignPartyCharacterDto(
                        character.Id,
                        character.CharacterId,
                        character.ControlledByUserId,
                        character.AssignedAtUtc ) )
                    .ToArray(),
                new CampaignPartyStorageDto(
                    party.Storage.Id,
                    party.Storage.AccessPolicy,
                    party.Storage.CreatedAtUtc ) ) )
            .ToArray();

        return new CampaignDto(
            campaign.Id,
            userId,
            campaign.Name,
            campaign.Status,
            campaign.CreatedByUserId,
            campaign.CreatedAtUtc,
            campaign.ArchivedAtUtc,
            roles,
            members,
            parties );
    }
}
