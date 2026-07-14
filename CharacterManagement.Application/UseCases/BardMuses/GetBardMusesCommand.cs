using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.BardMuses;

public sealed class GetBardMusesCommand : IRequest<IReadOnlyCollection<BardMuseDto>>
{
}
