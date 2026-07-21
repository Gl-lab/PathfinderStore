using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record GetPendingCampaignInvitationsQuery( int UserId )
    : IRequest<IReadOnlyCollection<CampaignInvitationDto>>;
