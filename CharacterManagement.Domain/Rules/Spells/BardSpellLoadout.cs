namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed class BardSpellLoadout
{
    public IReadOnlyList<string> CantripIds { get; }
    public IReadOnlyList<string> SelectedRankOneSpellIds { get; }
    public string MuseGrantedSpellId { get; }

    internal BardSpellLoadout(
        IReadOnlyList<string> cantripIds,
        IReadOnlyList<string> selectedRankOneSpellIds,
        string museGrantedSpellId )
    {
        CantripIds = cantripIds.ToArray();
        SelectedRankOneSpellIds = selectedRankOneSpellIds.ToArray();
        MuseGrantedSpellId = museGrantedSpellId;
    }
}
