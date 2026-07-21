using FluentValidation;
using MediatR;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class CreateCampaignHandler : IRequestHandler<CreateCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCampaignCommand> _validator;
    private readonly TimeProvider _timeProvider;

    public CreateCampaignHandler(
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCampaignCommand> validator,
        TimeProvider timeProvider )
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _timeProvider = timeProvider;
    }

    public async Task<CampaignDto> Handle(
        CreateCampaignCommand request,
        CancellationToken cancellationToken )
    {
        await _validator.ValidateAndThrowAsync( request, cancellationToken );
        Campaign campaign = Campaign.Create(
            request.Name,
            request.UserId,
            _timeProvider.GetUtcNow() );
        _campaignRepository.Add( campaign );
        await _unitOfWork.Commit();
        return campaign.ToDto( request.UserId );
    }
}
