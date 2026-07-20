using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class FeatDefinitionDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public FeatCategory Category { get; set; }
    public int Level { get; set; }
    public IReadOnlyList<string> Traits { get; set; } = [];
    public FeatRarity Rarity { get; set; }
    public IReadOnlyList<string> Prerequisites { get; set; } = [];
    public string Summary { get; set; } = String.Empty;
    public IReadOnlyList<FeatDependencyType> DeferredDependencies { get; set; } = [];
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
}
