namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed class WitchSpellLoadout
{
    public IReadOnlyList<string> FamiliarCantripIds { get; }
    public IReadOnlyList<string> FamiliarRankOneSpellIds { get; }
    public string PatronGrantedSpellId { get; }
    public IReadOnlyList<string> PreparedCantripIds { get; }
    public IReadOnlyList<string> PreparedSpellIds { get; }
    public string FocusHexId { get; }

    internal WitchSpellLoadout(
        IReadOnlyList<string> familiarCantripIds,
        IReadOnlyList<string> familiarRankOneSpellIds,
        string patronGrantedSpellId,
        IReadOnlyList<string> preparedCantripIds,
        IReadOnlyList<string> preparedSpellIds,
        string focusHexId )
    {
        FamiliarCantripIds = familiarCantripIds.ToArray();
        FamiliarRankOneSpellIds = familiarRankOneSpellIds.ToArray();
        PatronGrantedSpellId = patronGrantedSpellId;
        PreparedCantripIds = preparedCantripIds.ToArray();
        PreparedSpellIds = preparedSpellIds.ToArray();
        FocusHexId = focusHexId;
    }
}
