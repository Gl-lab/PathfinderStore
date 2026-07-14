namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed class ClericSpellLoadout
{
    public IReadOnlyList<string> CantripIds { get; }
    public IReadOnlyList<string> PreparedSpellIds { get; }

    internal ClericSpellLoadout(
        IReadOnlyList<string> cantripIds,
        IReadOnlyList<string> preparedSpellIds )
    {
        ArgumentNullException.ThrowIfNull( cantripIds );
        ArgumentNullException.ThrowIfNull( preparedSpellIds );

        CantripIds = cantripIds.ToArray();
        PreparedSpellIds = preparedSpellIds.ToArray();
    }
}
