using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record CampaignDto(
    int Id,
    string Name,
    CampaignStatus Status,
    int CreatedByUserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ArchivedAtUtc,
    IReadOnlyCollection<CampaignMembershipRole> Roles );
