using MediatR;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public class DeleteCharacterCommand: IRequest
{
    public DeleteCharacterCommand(int deletedCharacterId)
    {
        DeletedCharacterId = deletedCharacterId;
    }

    public int DeletedCharacterId { get; }
}