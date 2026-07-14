using Pathfinder.CharacterManagement.Domain.Rules.Spells;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class ClericPreparedSpellDto
{
    public SpellDefinitionDto Spell { get; set; } = new SpellDefinitionDto();
    public IReadOnlyList<ClericSpellAccessSource> AccessSources { get; set; } = [];
}

public sealed class ClericSpellLoadoutDto
{
    public IReadOnlyList<SpellDefinitionDto> Cantrips { get; set; } = [];
    public IReadOnlyList<ClericPreparedSpellDto> PreparedSpells { get; set; } = [];
    public IReadOnlyList<SpellDefinitionDto> DivineFontSpells { get; set; } = [];
}

public sealed class ClericAvailableSpellDto
{
    public SpellDefinitionDto Spell { get; set; } = new SpellDefinitionDto();
    public SpellTradition EffectiveTradition { get; set; }
    public IReadOnlyList<ClericSpellAccessSource> AccessSources { get; set; } = [];
}

public sealed class ClericSpellOptionsDto
{
    public IReadOnlyList<ClericAvailableSpellDto> Cantrips { get; set; } = [];
    public IReadOnlyList<ClericAvailableSpellDto> RankOneSpells { get; set; } = [];
}
