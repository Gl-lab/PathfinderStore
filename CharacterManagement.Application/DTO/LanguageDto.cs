using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class LanguageDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Speakers { get; set; } = String.Empty;
    public LanguageRarity Rarity { get; set; }
    public LanguageCategory Category { get; set; }
    public SourceReference Source { get; set; } = SourceReference.Unknown;
}
