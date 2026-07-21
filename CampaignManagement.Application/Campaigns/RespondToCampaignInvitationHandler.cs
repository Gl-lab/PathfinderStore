using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class RespondToCampaignInvitationHandler
    : IRequestHandler<RespondToCampaignInvitationCommand, CampaignDto?>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public RespondToCampaignInvitationHandler(
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider )
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<CampaignDto?> Handle(
        RespondToCampaignInvitationCommand request,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _campaignRepository.GetByInvitationIdForUserAsync(
            request.InvitationId,
            request.UserId,
            cancellationToken );
        if ( campaign is null )
        {
            throw new CampaignManagementApplicationException( "Pending campaign invitation was not found." );
        }

        if ( request.Accept )
        {
            campaign.AcceptInvitation(
                request.InvitationId,
                request.UserId,
                _timeProvider.GetUtcNow() );
        }
        else
        {
            campaign.DeclineInvitation(
                request.InvitationId,
                request.UserId,
                _timeProvider.GetUtcNow() );
        }

        await _unitOfWork.Commit();
        return request.Accept ? campaign.ToDto( request.UserId ) : null;
    }
}
