using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class DruidicOrderBenefitDto
{
    public string Id { get; set; } = String.Empty;
    public DruidicOrderBenefitKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class DruidicOrderDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public ClassSkillGrantDto SkillGrant { get; set; } = new ClassSkillGrantDto();
    public IReadOnlyList<DruidicOrderBenefitDto> Benefits { get; set; } = [];
}

public sealed class DruidicOrderPackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public ClassSkillGrantDto SkillGrant { get; set; } = new ClassSkillGrantDto();
    public IReadOnlyList<DruidicOrderBenefitDto> Benefits { get; set; } = [];
}
