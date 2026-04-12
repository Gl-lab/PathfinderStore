using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class CreateCharacterCommand : IRequest
{
    public CreateCharacterCommand( int userId, CreateCharacterRequestDto character )
    {
        UserId = userId;
        Character = character;
    }

    public int UserId { get; }
    public CreateCharacterRequestDto Character { get; }
}
