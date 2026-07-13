using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class RogueRacketDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public AbilityType? AlternativeKeyAbility { get; set; }
    public IReadOnlyList<RogueSkillGrantDto> SkillGrants { get; set; } = [];
    public IReadOnlyList<ProficiencyDto> ProficiencyGrants { get; set; } = [];
    public IReadOnlyList<RogueRacketEffectDto> Effects { get; set; } = [];
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class RogueSkillGrantDto
{
    public string Id { get; set; } = String.Empty;
    public string? TargetId { get; set; }
    public IReadOnlyList<string> Options { get; set; } = [];
    public bool RequiresChoice { get; set; }
}

public sealed class RogueRacketEffectDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
}

public sealed class RogueRacketPackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public AbilityType? AlternativeKeyAbility { get; set; }
    public IReadOnlyList<RogueRacketEffectDto> Effects { get; set; } = [];
}
