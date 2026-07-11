using Pathfinder.CharacterManagement.Application.DTO.Base;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public class AncestryDto : BaseDto
{
    public AncestryType Type { get; set; }
    public IReadOnlyList<AncestryBoostDto> AbilityBoosts { get; set; }
    public IReadOnlyList<AbilityType> AbilityFlaws { get; set; }
    public int BaseHitPoints { get; set; }
    public RaceSizeType Size { get; set; }
    public int BaseSpeed { get; set; }
    public bool Darkvision { get; set; }
    public bool LowLightVision { get; set; }
}

public class AncestryBoostDto
{
    public AbilityType? AbilityType { get; set; }
    public bool IsFree { get; set; }
}
