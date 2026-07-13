using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterTrainingDto
{
    public IReadOnlyList<CharacterSkillTrainingDto> Skills { get; set; } = [];
    public IReadOnlyList<CharacterLoreTrainingDto> Lore { get; set; } = [];
}

public sealed class CharacterSkillTrainingDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public AbilityType? KeyAbility { get; set; }
    public string SourceGrantId { get; set; } = String.Empty;
}

public sealed class CharacterLoreTrainingDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public AbilityType KeyAbility { get; set; } = AbilityType.Intelligence;
    public string SourceGrantId { get; set; } = String.Empty;
}
