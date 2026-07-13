using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Deities;

public sealed class GetDeitiesCommand : IRequest<IReadOnlyCollection<DeityDto>>
{
}
