namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class ClericFocusPoolDto
{
    public int MaximumFocusPoints { get; set; }
    public SpellDefinitionDto FocusSpell { get; set; } = new SpellDefinitionDto();
    public string SourceGrantId { get; set; } = String.Empty;
}
