using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterClassPackageDto
{
    public string ClassId { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public int BaseHitPoints { get; set; }
    public AbilityType KeyAbility { get; set; }
    public IReadOnlyList<CharacterClassRuleDto> Rules { get; set; } = [];
}
