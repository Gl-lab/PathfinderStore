using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterFeatDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public FeatCategory Category { get; set; }
    public int Level { get; set; }
    public IReadOnlyList<string> Traits { get; set; } = [];
    public IReadOnlyList<string> Prerequisites { get; set; } = [];
    public string Summary { get; set; } = String.Empty;
    public IReadOnlyList<FeatDependencyType> DeferredDependencies { get; set; } = [];
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public CharacterFeatAcquisitionType AcquisitionType { get; set; }
    public CharacterFeatSourceType SourceType { get; set; }
    public string SourceId { get; set; } = String.Empty;
}
