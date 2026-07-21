using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class ArchiveCampaignHandler : IRequestHandler<ArchiveCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public ArchiveCampaignHandler(
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider )
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<CampaignDto> Handle(
        ArchiveCampaignCommand request,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _campaignRepository.GetByIdForUserAsync(
            request.CampaignId,
            request.UserId,
            cancellationToken );
        if ( campaign is null )
        {
            throw new CampaignManagementApplicationException( "Campaign was not found." );
        }

        campaign.Archive( request.UserId, _timeProvider.GetUtcNow() );
        await _unitOfWork.Commit();
        return campaign.ToDto( request.UserId );
    }
}
