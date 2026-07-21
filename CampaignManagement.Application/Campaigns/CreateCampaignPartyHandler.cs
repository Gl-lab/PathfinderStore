using FluentValidation;
using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class CreateCampaignPartyHandler : IRequestHandler<CreateCampaignPartyCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCampaignPartyCommand> _validator;
    private readonly TimeProvider _timeProvider;

    public CreateCampaignPartyHandler(
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCampaignPartyCommand> validator,
        TimeProvider timeProvider )
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _timeProvider = timeProvider;
    }

    public async Task<CampaignDto> Handle(
        CreateCampaignPartyCommand request,
        CancellationToken cancellationToken )
    {
        await _validator.ValidateAndThrowAsync( request, cancellationToken );
        Campaign? campaign = await _campaignRepository.GetByIdForUserAsync(
            request.CampaignId,
            request.UserId,
            cancellationToken );
        if ( campaign is null )
        {
            throw new CampaignManagementApplicationException( "Campaign was not found." );
        }

        campaign.CreateParty( request.UserId, request.Name, _timeProvider.GetUtcNow() );
        await _unitOfWork.Commit();
        return campaign.ToDto( request.UserId );
    }
}
