using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.RogueRackets;

public sealed class GetRogueRacketsCommand : IRequest<IReadOnlyCollection<RogueRacketDto>>
{
}
