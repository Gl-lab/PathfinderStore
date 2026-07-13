using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.CharacterClasses;

public sealed class GetCharacterClassesCommand : IRequest<IReadOnlyCollection<CharacterClassDto>>
{
}
