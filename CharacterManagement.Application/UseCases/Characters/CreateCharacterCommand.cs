using CharacterManagement.Application.DTO;
using MediatR;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class CreateCharacterCommand : IRequest, IRequest<Task>
{
    public CreateCharacterCommand(CharacterDto character)
    {
        Character = character;
    }

    public CharacterDto Character { get; }
}