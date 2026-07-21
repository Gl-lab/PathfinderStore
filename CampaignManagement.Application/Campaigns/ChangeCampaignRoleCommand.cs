using MediatR;
using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record ChangeCampaignRoleCommand(
    int ActingUserId,
    int CampaignId,
    int MemberUserId,
    CampaignMembershipRole Role,
    bool Assign ) : IRequest<CampaignDto>;
