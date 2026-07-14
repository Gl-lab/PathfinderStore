using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class SetCharacterGenderRequestDto
{
    public CharacterGender Gender { get; set; }
}
