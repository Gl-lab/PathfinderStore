using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Ancestries;

public class GetAncestriesCommand : IRequest<IReadOnlyCollection<AncestryDto>>
{
}
