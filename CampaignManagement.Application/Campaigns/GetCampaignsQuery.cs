using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record GetCampaignsQuery( int UserId ) : IRequest<IReadOnlyCollection<CampaignDto>>;
