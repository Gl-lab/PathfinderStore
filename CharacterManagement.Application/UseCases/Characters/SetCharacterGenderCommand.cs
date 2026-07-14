using MediatR;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class SetCharacterGenderCommand : IRequest
{
    public SetCharacterGenderCommand(
        int userId,
        int characterId,
        CharacterGender gender )
    {
        UserId = userId;
        CharacterId = characterId;
        Gender = gender;
    }

    public int UserId { get; }
    public int CharacterId { get; }
    public CharacterGender Gender { get; }
}
