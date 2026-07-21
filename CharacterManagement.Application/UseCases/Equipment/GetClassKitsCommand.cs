using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Equipment;

public sealed class GetClassKitsCommand : IRequest<IReadOnlyCollection<ClassKitDto>>
{
}
