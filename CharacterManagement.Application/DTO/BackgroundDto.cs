using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class BackgroundDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public IReadOnlyList<AbilityType> RestrictedBoostOptions { get; set; } = [];
    public int FreeBoostCount { get; set; }
    public IReadOnlyList<BackgroundGrantDto> Grants { get; set; } = [];
}

public sealed class BackgroundGrantDto
{
    public string Id { get; set; } = String.Empty;
    public BackgroundGrantKind Kind { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
    public bool RequiresChoice { get; set; }
    public IReadOnlyList<string> Options { get; set; } = [];
    public IReadOnlyList<BackgroundDependencyType> DeferredDependencies { get; set; } = [];
}
