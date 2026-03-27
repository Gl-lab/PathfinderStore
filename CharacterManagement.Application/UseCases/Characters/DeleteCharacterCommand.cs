using MediatR;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class DeleteCharacterCommand : IRequest
{
    public DeleteCharacterCommand( int userId, int deletedCharacterId )
    {
        UserId = userId;
        DeletedCharacterId = deletedCharacterId;
    }

    public int UserId { get; }
    public int DeletedCharacterId { get; }
}
