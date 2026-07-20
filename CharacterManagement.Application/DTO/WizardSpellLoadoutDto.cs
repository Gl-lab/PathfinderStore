namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class WizardSpellLoadoutDto
{
    public IReadOnlyList<SpellDefinitionDto> SpellbookCantrips { get; set; } = [];
    public IReadOnlyList<SpellDefinitionDto> SpellbookRankOneSpells { get; set; } = [];
    public SpellDefinitionDto? CurriculumCantrip { get; set; }
    public IReadOnlyList<SpellDefinitionDto> CurriculumRankOneSpells { get; set; } = [];
    public IReadOnlyList<SpellDefinitionDto> PreparedCantrips { get; set; } = [];
    public IReadOnlyList<SpellDefinitionDto> PreparedRankOneSpells { get; set; } = [];
    public SpellDefinitionDto? PreparedCurriculumCantrip { get; set; }
    public SpellDefinitionDto? PreparedCurriculumRankOneSpell { get; set; }
    public int BaseRankOneSpellSlotCount { get; set; } = 2;
    public int CurriculumRankOneSpellSlotCount { get; set; }
}

public sealed class WizardSchoolMagicDto
{
    public int MaximumFocusPoints { get; set; }
    public int DrainBondedItemUsesPerDay { get; set; }
    public SpellDefinitionDto? InitialSchoolSpell { get; set; }
    public string SourceGrantId { get; set; } = String.Empty;
}
