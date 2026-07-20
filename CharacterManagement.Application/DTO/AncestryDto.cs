using Pathfinder.CharacterManagement.Application.DTO.Base;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class AncestryDto : BaseDto
{
    public AncestryType Type { get; set; }
    public string Name { get; set; }
    public SourceReferenceDto Source { get; set; }
    public IReadOnlyList<AncestryBoostDto> AbilityBoosts { get; set; }
    public IReadOnlyList<AbilityType> AbilityFlaws { get; set; }
    public int BaseHitPoints { get; set; }
    public RaceSizeType Size { get; set; }
    public int BaseSpeed { get; set; }
    public VisionType Vision { get; set; }
    public bool Darkvision { get; set; }
    public bool LowLightVision { get; set; }
    public IReadOnlyList<string> StartingLanguageIds { get; set; }
    public AdditionalLanguageRuleDto? AdditionalLanguageRule { get; set; }
    public IReadOnlyList<GrantedItemDto> GrantedItems { get; set; }
    public IReadOnlyList<GrantedRuleDto> GrantedRules { get; set; }
    public IReadOnlyList<HeritageDto> Heritages { get; set; }
    public IReadOnlyList<AncestryFeatDto> AncestryFeats { get; set; }
}

public sealed class SourceReferenceDto
{
    public string Book { get; set; }
    public int Page { get; set; }
}

public sealed class AdditionalLanguageRuleDto
{
    public AdditionalLanguageRuleType Type { get; set; }
    public IReadOnlyList<string> AllowedLanguageIds { get; set; }
    public bool AllowsCommonLanguages { get; set; }
    public bool AllowsAccessLanguages { get; set; }
}

public sealed class GrantedItemDto
{
    public string ItemId { get; set; }
    public int Quantity { get; set; }
    public SourceReferenceDto Source { get; set; }
}

public sealed class GrantedRuleDto
{
    public string RuleId { get; set; }
    public AncestryEffectKind EffectKind { get; set; }
    public string Summary { get; set; }
}

public sealed class HeritageDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public SourceReferenceDto Source { get; set; }
    public AncestryChoiceRarity Rarity { get; set; }
    public IReadOnlyList<string> Restrictions { get; set; }
    public IReadOnlyList<string> IncompatibleChoiceIds { get; set; }
    public IReadOnlyList<AncestryEffectDto> Effects { get; set; }
    public IReadOnlyList<AncestryDependencyType> DeferredDependencies { get; set; }
}

public sealed class AncestryFeatDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public SourceReferenceDto Source { get; set; }
    public int Level { get; set; }
    public AncestryChoiceRarity Rarity { get; set; }
    public IReadOnlyList<string> Prerequisites { get; set; }
    public IReadOnlyList<string> Restrictions { get; set; }
    public IReadOnlyList<string> IncompatibleChoiceIds { get; set; }
    public IReadOnlyList<AncestryEffectDto> Effects { get; set; }
    public IReadOnlyList<AncestryDependencyType> DeferredDependencies { get; set; }
}

public sealed class AncestryEffectDto
{
    public string EffectId { get; set; }
    public AncestryEffectKind EffectKind { get; set; }
    public string Summary { get; set; }
    public VisionType? VisionOverride { get; set; }
    public int? BaseHitPointsOverride { get; set; }
}

public sealed class AncestryBoostDto
{
    public AbilityType? AbilityType { get; set; }
    public bool IsFree { get; set; }
}
