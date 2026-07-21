using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed record ChangeHitPointsCommand(
    int UserId,
    int CharacterId,
    HitPointOperation Operation,
    int Amount ) : IRequest<CharacterHitPointStateDto>;
