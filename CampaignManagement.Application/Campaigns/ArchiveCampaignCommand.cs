using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record ArchiveCampaignCommand( int UserId, int CampaignId ) : IRequest<CampaignDto>;
