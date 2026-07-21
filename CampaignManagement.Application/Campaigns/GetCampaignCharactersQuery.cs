using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record GetCampaignCharactersQuery(
    int UserId ) : IRequest<IReadOnlyCollection<CampaignCharacterReference>>;
