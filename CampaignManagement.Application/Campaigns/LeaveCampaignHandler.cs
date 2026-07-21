using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class LeaveCampaignHandler : IRequestHandler<LeaveCampaignCommand>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LeaveCampaignHandler( ICampaignRepository campaignRepository, IUnitOfWork unitOfWork )
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle( LeaveCampaignCommand request, CancellationToken cancellationToken )
    {
        Campaign? campaign = await _campaignRepository.GetByIdForUserAsync(
            request.CampaignId,
            request.UserId,
            cancellationToken );
        if ( campaign is null )
        {
            throw new CampaignManagementApplicationException( "Campaign was not found." );
        }

        campaign.Leave( request.UserId );
        await _unitOfWork.Commit();
    }
}
