namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record CampaignInvitationDto(
    int Id,
    int CampaignId,
    string CampaignName,
    int InvitedByUserId,
    DateTimeOffset CreatedAtUtc );
