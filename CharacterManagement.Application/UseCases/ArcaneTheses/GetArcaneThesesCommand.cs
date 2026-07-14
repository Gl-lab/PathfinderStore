using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.ArcaneTheses;

public sealed class GetArcaneThesesCommand
    : IRequest<IReadOnlyCollection<ArcaneThesisDto>>
{
}
