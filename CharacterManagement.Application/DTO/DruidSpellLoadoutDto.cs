namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class DruidSpellLoadoutDto
{
    public IReadOnlyList<SpellDefinitionDto> Cantrips { get; set; } = [];
    public IReadOnlyList<SpellDefinitionDto> PreparedSpells { get; set; } = [];
}

public sealed class DruidFocusPoolDto
{
    public int MaximumFocusPoints { get; set; }
    public SpellDefinitionDto FocusSpell { get; set; } = new SpellDefinitionDto();
    public string SourceGrantId { get; set; } = String.Empty;
}
