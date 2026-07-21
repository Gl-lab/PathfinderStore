using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record AssignCampaignPartyCharacterCommand(
    int UserId,
    int CampaignId,
    int CharacterId,
    int ControlledByUserId ) : IRequest<CampaignDto>;
