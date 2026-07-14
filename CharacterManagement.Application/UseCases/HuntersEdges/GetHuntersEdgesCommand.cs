using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.HuntersEdges;

public sealed class GetHuntersEdgesCommand : IRequest<IReadOnlyCollection<HuntersEdgeDto>>
{
}
