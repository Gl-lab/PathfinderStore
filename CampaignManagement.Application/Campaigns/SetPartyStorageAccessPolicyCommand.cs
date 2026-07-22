using MediatR;
using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record SetPartyStorageAccessPolicyCommand(
    int UserId,
    int CampaignId,
    CampaignPartyStorageAccessPolicy AccessPolicy ) : IRequest<CampaignDto>;
