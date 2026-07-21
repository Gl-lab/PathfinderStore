using MediatR;
using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class GetPendingCampaignInvitationsHandler
    : IRequestHandler<GetPendingCampaignInvitationsQuery, IReadOnlyCollection<CampaignInvitationDto>>
{
    private readonly ICampaignRepository _campaignRepository;

    public GetPendingCampaignInvitationsHandler( ICampaignRepository campaignRepository )
    {
        _campaignRepository = campaignRepository;
    }

    public async Task<IReadOnlyCollection<CampaignInvitationDto>> Handle(
        GetPendingCampaignInvitationsQuery request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<Campaign> campaigns =
            await _campaignRepository.GetPendingInvitationsForUserAsync(
                request.UserId,
                cancellationToken );
        return campaigns
            .SelectMany( campaign => campaign.Invitations
                .Where( invitation =>
                    ( invitation.InvitedUserId == request.UserId ) &&
                    ( invitation.Status == CampaignInvitationStatus.Pending ) )
                .Select( invitation => new CampaignInvitationDto(
                    invitation.Id,
                    campaign.Id,
                    campaign.Name,
                    invitation.InvitedByUserId,
                    invitation.CreatedAtUtc ) ) )
            .OrderByDescending( invitation => invitation.CreatedAtUtc )
            .ToArray();
    }
}
