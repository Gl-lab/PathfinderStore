using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class WitchPatronBenefitDto
{
    public string Id { get; set; } = String.Empty;
    public WitchPatronBenefitKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class WitchPatronDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public SpellTradition SpellTradition { get; set; }
    public ClassSkillGrantDto SkillGrant { get; set; } = new ClassSkillGrantDto();
    public IReadOnlyList<WitchPatronBenefitDto> Benefits { get; set; } = [];
}

public sealed class WitchPatronPackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SpellTradition SpellTradition { get; set; }
    public ClassSkillGrantDto SkillGrant { get; set; } = new ClassSkillGrantDto();
    public IReadOnlyList<WitchPatronBenefitDto> Benefits { get; set; } = [];
    public WitchPatronBenefitDto SelectedFamiliarSpell { get; set; } = new WitchPatronBenefitDto();
}
