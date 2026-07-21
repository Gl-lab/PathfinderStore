using FluentValidation;
using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class InviteCampaignMemberHandler : IRequestHandler<InviteCampaignMemberCommand>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignUserDirectory _userDirectory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<InviteCampaignMemberCommand> _validator;
    private readonly TimeProvider _timeProvider;

    public InviteCampaignMemberHandler(
        ICampaignRepository campaignRepository,
        ICampaignUserDirectory userDirectory,
        IUnitOfWork unitOfWork,
        IValidator<InviteCampaignMemberCommand> validator,
        TimeProvider timeProvider )
    {
        _campaignRepository = campaignRepository;
        _userDirectory = userDirectory;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _timeProvider = timeProvider;
    }

    public async Task Handle(
        InviteCampaignMemberCommand request,
        CancellationToken cancellationToken )
    {
        await _validator.ValidateAndThrowAsync( request, cancellationToken );
        Campaign? campaign = await _campaignRepository.GetByIdForUserAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        if ( campaign is null )
        {
            throw new CampaignManagementApplicationException( "Campaign was not found." );
        }

        int? invitedUserId = await _userDirectory.FindUserIdByNameAsync(
            request.UserName.Trim(),
            cancellationToken );
        if ( invitedUserId is null )
        {
            throw new CampaignManagementApplicationException( "Invited user was not found." );
        }

        campaign.Invite( request.ActingUserId, invitedUserId.Value, _timeProvider.GetUtcNow() );
        await _unitOfWork.Commit();
    }
}
