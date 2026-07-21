using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record InviteCampaignMemberCommand(
    int ActingUserId,
    int CampaignId,
    string UserName ) : IRequest;
