using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed class WizardSpellLoadout
{
    public IReadOnlyList<string> SpellbookCantripIds { get; }
    public IReadOnlyList<string> SpellbookRankOneSpellIds { get; }
    public string? CurriculumCantripId { get; }
    public IReadOnlyList<string> CurriculumRankOneSpellIds { get; }
    public IReadOnlyList<string> PreparedCantripIds { get; }
    public IReadOnlyList<string> PreparedRankOneSpellIds { get; }
    public string? PreparedCurriculumCantripId { get; }
    public string? PreparedCurriculumRankOneSpellId { get; }
    public string InitialSchoolSpellId { get; }
    public int MaximumFocusPoints => 1;
    public int DrainBondedItemUsesPerDay => 1;

    internal WizardSpellLoadout(
        IReadOnlyList<string> spellbookCantripIds,
        IReadOnlyList<string> spellbookRankOneSpellIds,
        string? curriculumCantripId,
        IReadOnlyList<string> curriculumRankOneSpellIds,
        IReadOnlyList<string> preparedCantripIds,
        IReadOnlyList<string> preparedRankOneSpellIds,
        string? preparedCurriculumCantripId,
        string? preparedCurriculumRankOneSpellId,
        string initialSchoolSpellId )
    {
        SpellbookCantripIds = spellbookCantripIds.ToArray();
        SpellbookRankOneSpellIds = spellbookRankOneSpellIds.ToArray();
        CurriculumCantripId = curriculumCantripId;
        CurriculumRankOneSpellIds = curriculumRankOneSpellIds.ToArray();
        PreparedCantripIds = preparedCantripIds.ToArray();
        PreparedRankOneSpellIds = preparedRankOneSpellIds.ToArray();
        PreparedCurriculumCantripId = preparedCurriculumCantripId;
        PreparedCurriculumRankOneSpellId = preparedCurriculumRankOneSpellId;
        InitialSchoolSpellId = initialSchoolSpellId;
    }
}
