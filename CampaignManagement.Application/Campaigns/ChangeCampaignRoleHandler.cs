using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class ChangeCampaignRoleHandler
    : IRequestHandler<ChangeCampaignRoleCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public ChangeCampaignRoleHandler(
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider )
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<CampaignDto> Handle(
        ChangeCampaignRoleCommand request,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _campaignRepository.GetByIdForUserAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        if ( campaign is null )
        {
            throw new CampaignManagementApplicationException( "Campaign was not found." );
        }

        if ( request.Assign )
        {
            campaign.AssignRole(
                request.ActingUserId,
                request.MemberUserId,
                request.Role,
                _timeProvider.GetUtcNow() );
        }
        else
        {
            campaign.RevokeRole(
                request.ActingUserId,
                request.MemberUserId,
                request.Role );
        }

        await _unitOfWork.Commit();
        return campaign.ToDto( request.ActingUserId );
    }
}
