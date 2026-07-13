using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericDoctrines;

public sealed class GetClericDoctrinesCommand : IRequest<IReadOnlyCollection<ClericDoctrineDto>>
{
}
