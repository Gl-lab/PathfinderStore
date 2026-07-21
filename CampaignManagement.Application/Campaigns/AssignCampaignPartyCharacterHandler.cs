using MediatR;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class AssignCampaignPartyCharacterHandler
    : IRequestHandler<AssignCampaignPartyCharacterCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignCharacterDirectory _characterDirectory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public AssignCampaignPartyCharacterHandler(
        ICampaignRepository campaignRepository,
        ICampaignCharacterDirectory characterDirectory,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider )
    {
        _campaignRepository = campaignRepository;
        _characterDirectory = characterDirectory;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<CampaignDto> Handle(
        AssignCampaignPartyCharacterCommand request,
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

        CampaignCharacterReference? character = await _characterDirectory.GetOwnedCharacterAsync(
            request.CharacterId,
            request.ControlledByUserId,
            cancellationToken );
        if ( character is null )
        {
            throw new CampaignManagementApplicationException(
                "Character was not found for the selected controller." );
        }

        campaign.AssignCharacterToActiveParty(
            request.UserId,
            character.Id,
            request.ControlledByUserId,
            _timeProvider.GetUtcNow() );
        await _unitOfWork.Commit();
        return campaign.ToDto( request.UserId );
    }
}
