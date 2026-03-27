using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class GetCharacterByIdCommand : IRequest<CharacterDto>
{
    public GetCharacterByIdCommand( int userId, int characterId )
    {
        UserId = userId;
        CharacterId = characterId;
    }

    public int UserId { get; }
    public int CharacterId { get; }
}
