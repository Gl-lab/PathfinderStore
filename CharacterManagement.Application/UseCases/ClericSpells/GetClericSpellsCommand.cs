using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericSpells;

public sealed class GetClericSpellsCommand : IRequest<IReadOnlyCollection<SpellDefinitionDto>>
{
}
