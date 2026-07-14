using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class SpellDefinitionDto
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public int Rank { get; set; }
    public SpellKind Kind { get; set; }
    public IReadOnlyList<SpellTradition> Traditions { get; set; } = [];
    public IReadOnlyList<string> Traits { get; set; } = [];
    public SpellRarity Rarity { get; set; }
    public SourceReferenceDto Source { get; set; } = new SourceReferenceDto();
}
