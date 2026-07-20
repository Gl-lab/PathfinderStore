using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed record FinalizeCharacterCommand(
    int UserId,
    int CharacterId ) : IRequest<CharacterCreationStateDto>;
