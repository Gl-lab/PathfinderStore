namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class WitchSpellLoadoutDto
{
    public IReadOnlyList<SpellDefinitionDto> FamiliarCantrips { get; set; } = [];
    public IReadOnlyList<SpellDefinitionDto> FamiliarRankOneSpells { get; set; } = [];
    public SpellDefinitionDto? PatronGrantedSpell { get; set; }
    public IReadOnlyList<SpellDefinitionDto> PreparedCantrips { get; set; } = [];
    public IReadOnlyList<SpellDefinitionDto> PreparedSpells { get; set; } = [];
    public int RankOneSpellSlotCount { get; set; } = 2;
}

public sealed class WitchHexPackageDto
{
    public int MaximumFocusPoints { get; set; }
    public SpellDefinitionDto? PatronHexCantrip { get; set; }
    public SpellDefinitionDto? FocusHex { get; set; }
    public string SourceGrantId { get; set; } = String.Empty;
}
