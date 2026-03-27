using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public class CreateCharacterRequestDto
{
    public string Name { get; set; }
    public AncestryType AncestryType { get; set; }
    public IReadOnlyList<AbilityType> FreeBoosts { get; set; } = [];
}
