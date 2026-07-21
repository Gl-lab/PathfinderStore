using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class GetCampaignsHandler : IRequestHandler<GetCampaignsQuery, IReadOnlyCollection<CampaignDto>>
{
    private readonly ICampaignRepository _campaignRepository;

    public GetCampaignsHandler( ICampaignRepository campaignRepository )
    {
        _campaignRepository = campaignRepository;
    }

    public async Task<IReadOnlyCollection<CampaignDto>> Handle(
        GetCampaignsQuery request,
        CancellationToken cancellationToken )
    {
        if ( request.UserId <= 0 )
        {
            throw new CampaignManagementApplicationException( "User id must be greater than zero." );
        }

        IReadOnlyCollection<Campaign> campaigns = await _campaignRepository.GetForUserAsync(
            request.UserId,
            cancellationToken );
        return campaigns
            .Select( campaign => campaign.ToDto( request.UserId ) )
            .ToArray();
    }
}
