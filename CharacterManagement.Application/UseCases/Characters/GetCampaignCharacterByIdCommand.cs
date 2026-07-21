using MediatR;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed record GetCampaignCharacterByIdCommand(
    int UserId,
    int CampaignId,
    int CharacterId ) : IRequest<CampaignCharacterDto>;
