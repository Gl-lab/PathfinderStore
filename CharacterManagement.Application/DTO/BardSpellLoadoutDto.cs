namespace Pathfinder.CharacterManagement.Application.DTO;

public enum BardRepertoireSpellSource
{
    Selected,
    MuseGranted
}

public sealed class BardRepertoireSpellDto
{
    public SpellDefinitionDto Spell { get; set; } = new SpellDefinitionDto();
    public BardRepertoireSpellSource Source { get; set; }
    public string SourceGrantId { get; set; } = String.Empty;
}

public sealed class BardSpellLoadoutDto
{
    public IReadOnlyList<SpellDefinitionDto> Cantrips { get; set; } = [];
    public IReadOnlyList<BardRepertoireSpellDto> RankOneRepertoire { get; set; } = [];
    public int RankOneSpellSlotCount { get; set; }
}

public sealed class BardCompositionPackageDto
{
    public int MaximumFocusPoints { get; set; }
    public SpellDefinitionDto CompositionCantrip { get; set; } = new SpellDefinitionDto();
    public SpellDefinitionDto FocusSpell { get; set; } = new SpellDefinitionDto();
    public string SourceGrantId { get; set; } = String.Empty;
}
