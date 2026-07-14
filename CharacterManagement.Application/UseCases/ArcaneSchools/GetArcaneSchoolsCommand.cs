using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.ArcaneSchools;

public sealed class GetArcaneSchoolsCommand : IRequest<IReadOnlyCollection<ArcaneSchoolDto>>
{
}
