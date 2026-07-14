using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class ArcaneThesisEffectDto
{
    public string Id { get; set; } = String.Empty;
    public ArcaneThesisEffectKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
    public IReadOnlyList<int> MilestoneLevels { get; set; } = [];
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public class ArcaneThesisPackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public IReadOnlyList<ArcaneThesisEffectDto> Effects { get; set; } = [];
}

public sealed class ArcaneThesisDto : ArcaneThesisPackageDto
{
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
}
