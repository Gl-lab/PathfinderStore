using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class ArcaneSchoolCurriculumSpellDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public int Rank { get; set; }
    public bool IsUncommon { get; set; }
}

public sealed class ArcaneSchoolBenefitDto
{
    public string Id { get; set; } = String.Empty;
    public ArcaneSchoolBenefitKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public class ArcaneSchoolPackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public bool HasCurriculum { get; set; }
    public IReadOnlyList<ArcaneSchoolCurriculumSpellDto> CurriculumSpells { get; set; } = [];
    public IReadOnlyList<ArcaneSchoolBenefitDto> Benefits { get; set; } = [];
}

public sealed class ArcaneSchoolDto : ArcaneSchoolPackageDto
{
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
}
