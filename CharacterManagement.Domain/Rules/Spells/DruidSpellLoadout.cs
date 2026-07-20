namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed class DruidSpellLoadout
{
    public IReadOnlyList<string> CantripIds { get; }
    public IReadOnlyList<string> PreparedSpellIds { get; }

    internal DruidSpellLoadout(
        IReadOnlyList<string> cantripIds,
        IReadOnlyList<string> preparedSpellIds )
    {
        CantripIds = cantripIds.ToArray();
        PreparedSpellIds = preparedSpellIds.ToArray();
    }
}
