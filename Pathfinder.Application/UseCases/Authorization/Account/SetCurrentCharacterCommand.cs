using MediatR;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class SetCurrentCharacterCommand : IRequest
{
    public SetCurrentCharacterCommand(int characterId)
    {
        CharacterId = characterId;
    }

    public int CharacterId { get; }
}