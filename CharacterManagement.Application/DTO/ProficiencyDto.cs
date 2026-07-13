using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class ProficiencyDto
{
    public string TargetId { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public ProficiencyCategory Category { get; set; }
    public ProficiencyRank Rank { get; set; }
    public string SourceGrantId { get; set; } = String.Empty;
}
