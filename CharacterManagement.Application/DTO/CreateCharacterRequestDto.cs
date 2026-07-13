using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public class CreateCharacterRequestDto
{
    public string Name { get; set; }
    public string? Concept { get; set; }
    public int? Age { get; set; }
    public AncestryType AncestryType { get; set; }
    public string HeritageId { get; set; }
    public string AncestryFeatId { get; set; }
    public IReadOnlyList<AbilityType> FreeBoosts { get; set; } = [];
    public string BackgroundId { get; set; } = String.Empty;
    public AbilityType? BackgroundRestrictedBoost { get; set; }
    public AbilityType? BackgroundFreeBoost { get; set; }
    public string ClassId { get; set; } = String.Empty;
    public AbilityType? ClassKeyAbility { get; set; }
}
