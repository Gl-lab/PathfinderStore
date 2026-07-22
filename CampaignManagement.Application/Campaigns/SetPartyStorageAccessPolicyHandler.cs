using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class SetPartyStorageAccessPolicyHandler
    : IRequestHandler<SetPartyStorageAccessPolicyCommand, CampaignDto>
{
    private readonly ICampaignRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetPartyStorageAccessPolicyHandler(
        ICampaignRepository repository,
        IUnitOfWork unitOfWork )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CampaignDto> Handle(
        SetPartyStorageAccessPolicyCommand request,
        CancellationToken cancellationToken )
    {
        Campaign campaign = await _repository.GetByIdForUserAsync(
            request.CampaignId,
            request.UserId,
            cancellationToken ) ?? throw new CampaignManagementApplicationException(
            "Campaign was not found." );
        campaign.SetActivePartyStorageAccessPolicy( request.UserId, request.AccessPolicy );
        await _unitOfWork.Commit();
        return campaign.ToDto( request.UserId );
    }
}
