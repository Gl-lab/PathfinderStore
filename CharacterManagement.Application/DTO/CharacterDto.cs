using Pathfinder.CharacterManagement.Application.DTO.Base;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public class CharacterDto : BaseDto
{
    public string Name { get; set; }
    public string? Concept { get; set; }
    public int? Age { get; set; }
    public CharacterGender Gender { get; set; }
    public string AvatarId { get; set; }
    public string AvatarPath { get; set; }
    public AncestryType AncestryType { get; set; }
    public CharacterAncestryPackageDto? AncestryPackage { get; set; }
    public CharacterBackgroundPackageDto? BackgroundPackage { get; set; }
    public CharacterClassPackageDto? ClassPackage { get; set; }
    public IReadOnlyList<AbilityType> FinalFreeBoosts { get; set; } = [];
    public CharacterDerivedStatisticsDto? DerivedStatistics { get; set; }
    public CharacterTrainingDto Training { get; set; } = new CharacterTrainingDto();
    public IReadOnlyList<ProficiencyDto> Proficiencies { get; set; } = [];
    public virtual BackpackDto? Backpack { get; set; }
    public virtual GroupCharacteristicDto Characteristics { get; set; }
}

public class BackpackDto : BaseDto
{
    public WalletDto Wallet { get; init; }
}

public class WalletDto : BaseDto
{
    public int Balance { get; set; }
}
