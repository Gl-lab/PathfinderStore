using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record CreateCampaignCommand( int UserId, string Name ) : IRequest<CampaignDto>;
