using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterAncestryPackageDto
{
    public string? SelectedHeritageId { get; set; }
    public string? SelectedAncestryFeatId { get; set; }
    public VisionType EffectiveVision { get; set; }
    public int EffectiveBaseHitPoints { get; set; }
    public IReadOnlyList<string> StartingLanguageIds { get; set; }
    public AdditionalLanguageRuleDto? AdditionalLanguageRule { get; set; }
    public IReadOnlyList<GrantedItemDto> GrantedItems { get; set; }
    public IReadOnlyList<GrantedRuleDto> GrantedRules { get; set; }
    public IReadOnlyList<AncestryEffectDto> SelectedEffects { get; set; }
    public IReadOnlyList<AncestryDependencyType> DeferredDependencies { get; set; }
}
