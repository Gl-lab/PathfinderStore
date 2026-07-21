using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record RespondToCampaignInvitationCommand(
    int UserId,
    int InvitationId,
    bool Accept ) : IRequest<CampaignDto?>;
