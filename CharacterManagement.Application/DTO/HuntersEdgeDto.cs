using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class HuntersEdgeEffectDto
{
    public string Id { get; set; } = String.Empty;
    public HuntersEdgeEffectKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
}

public sealed class HuntersEdgeDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public IReadOnlyList<HuntersEdgeEffectDto> Effects { get; set; } = [];
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class HuntersEdgePackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public IReadOnlyList<HuntersEdgeEffectDto> Effects { get; set; } = [];
}
