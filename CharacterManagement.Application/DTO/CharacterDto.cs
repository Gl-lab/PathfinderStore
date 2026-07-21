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
    public CharacterCreationStatus CreationStatus { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public AncestryType AncestryType { get; set; }
    public CharacterAncestryPackageDto? AncestryPackage { get; set; }
    public CharacterBackgroundPackageDto? BackgroundPackage { get; set; }
    public CharacterClassPackageDto? ClassPackage { get; set; }
    public IReadOnlyList<AbilityType> FinalFreeBoosts { get; set; } = [];
    public CharacterDerivedStatisticsDto? DerivedStatistics { get; set; }
    public CharacterTrainingDto Training { get; set; } = new CharacterTrainingDto();
    public IReadOnlyList<ProficiencyDto> Proficiencies { get; set; } = [];
    public IReadOnlyList<CharacterFeatDto> Feats { get; set; } = [];
    public CharacterCompletionDto Completion { get; set; } = new CharacterCompletionDto();
    public CharacterStartingEquipmentDto? StartingEquipment { get; set; }
    public virtual BackpackDto? Backpack { get; set; }
    public virtual GroupCharacteristicDto Characteristics { get; set; }
}

public sealed class CharacterStartingEquipmentDto
{
    public string ClassKitId { get; set; } = String.Empty;
    public IReadOnlyList<string> SelectedOptionIds { get; set; } = [];
    public IReadOnlyList<CharacterEquipmentLineDto> Items { get; set; } = [];
    public int TotalPriceCopper { get; set; }
    public int RemainingWealthCopper { get; set; }
    public int TotalBulkTenths { get; set; }
    public int EncumberedAtBulkTenths { get; set; }
    public int MaximumBulkTenths { get; set; }
    public bool IsEncumbered { get; set; }
    public bool ExceedsMaximumBulk { get; set; }
}

public sealed class CharacterEquipmentLineDto
{
    public CharacterEquipmentDefinitionDto Definition { get; set; } = new CharacterEquipmentDefinitionDto();
    public int PurchaseQuantity { get; set; }
    public int UnitQuantity { get; set; }
    public int EquippedQuantity { get; set; }
    public string? ProficiencyTargetId { get; set; }
    public ProficiencyRank ProficiencyRank { get; set; }
}

public sealed class CharacterEquipmentDefinitionDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public EquipmentCategory Category { get; set; }
    public int PriceCopper { get; set; }
    public int BulkTenths { get; set; }
    public int UnitsPerPurchase { get; set; }
}

public class BackpackDto : BaseDto
{
    public WalletDto Wallet { get; init; }
}

public class WalletDto : BaseDto
{
    public int Balance { get; set; }
}
