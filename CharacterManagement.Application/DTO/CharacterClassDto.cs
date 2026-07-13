using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterClassDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public int BaseHitPoints { get; set; }
    public IReadOnlyList<AbilityType> KeyAbilityOptions { get; set; } = [];
    public SpellTradition? SpellTradition { get; set; }
    public IReadOnlyList<CharacterClassRuleDto> Rules { get; set; } = [];
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class CharacterClassRuleDto
{
    public string Id { get; set; } = String.Empty;
    public CharacterClassRuleKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
    public bool RequiresChoice { get; set; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}
