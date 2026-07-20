using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Languages;

public sealed class GetLanguagesCommand : IRequest<IReadOnlyCollection<LanguageDto>>
{
}
