using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class BardMuseBenefitDto
{
    public string Id { get; set; } = String.Empty;
    public BardMuseBenefitKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class BardMuseDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public IReadOnlyList<BardMuseBenefitDto> Benefits { get; set; } = [];
}

public sealed class BardMusePackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public IReadOnlyList<BardMuseBenefitDto> Benefits { get; set; } = [];
}
