using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterBackgroundPackageDto
{
    public string BackgroundId { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public AbilityType RestrictedBoost { get; set; }
    public AbilityType FreeBoost { get; set; }
    public IReadOnlyList<BackgroundGrantDto> Grants { get; set; } = [];
}
