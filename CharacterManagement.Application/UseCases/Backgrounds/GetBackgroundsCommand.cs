using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Backgrounds;

public sealed class GetBackgroundsCommand : IRequest<IReadOnlyCollection<BackgroundDto>>
{
}
