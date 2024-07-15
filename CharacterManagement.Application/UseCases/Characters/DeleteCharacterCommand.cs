using MediatR;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class DeleteCharacterCommand: IRequest
{
    public DeleteCharacterCommand(int deletedCharacterId)
    {
        DeletedCharacterId = deletedCharacterId;
    }

    public int DeletedCharacterId { get; }
}