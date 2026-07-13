using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class ClericDoctrineDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
    public IReadOnlyList<ProficiencyDto> ProficiencyGrants { get; set; } = [];
    public IReadOnlyList<ClericDoctrineEffectDto> Effects { get; set; } = [];
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class ClericDoctrineEffectDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}

public sealed class ClericDoctrinePackageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public IReadOnlyList<ClericDoctrineEffectDto> Effects { get; set; } = [];
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; set; } = [];
}
