using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Skills;

public sealed class GetSkillsCommand : IRequest<IReadOnlyCollection<SkillDto>>
{
}
