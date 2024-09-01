using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public class CreateCharacterCommand : IRequest, IRequest<Task>
{
    public CreateCharacterCommand(CharacterDto character)
    {
        Character = character;
    }

    public CharacterDto Character { get; }
}