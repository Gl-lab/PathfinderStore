using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record LeaveCampaignCommand( int UserId, int CampaignId ) : IRequest;
