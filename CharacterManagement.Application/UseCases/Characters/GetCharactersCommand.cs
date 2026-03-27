using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class GetCharactersCommand : IRequest<IReadOnlyCollection<CharacterDto>>
{
    public GetCharactersCommand( int userId )
    {
        UserId = userId;
    }

    public int UserId { get; }
}
