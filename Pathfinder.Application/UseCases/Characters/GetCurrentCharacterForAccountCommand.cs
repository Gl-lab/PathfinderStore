using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Characters;

public class GetCurrentCharacterForAccountCommand : IRequest<CharacterDto>
{
}