using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record CreateCampaignPartyCommand(
    int UserId,
    int CampaignId,
    string Name ) : IRequest<CampaignDto>;
