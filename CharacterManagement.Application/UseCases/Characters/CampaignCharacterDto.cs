using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed record CampaignCharacterDto( CharacterDto Character, bool CanAct );
