using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed class ClericFocusPool
{
    public int MaximumFocusPoints { get; }
    public SpellDefinition FocusSpell { get; }
    public string SourceGrantId { get; }

    internal ClericFocusPool(
        int maximumFocusPoints,
        SpellDefinition focusSpell,
        string sourceGrantId )
    {
        MaximumFocusPoints = maximumFocusPoints;
        FocusSpell = focusSpell;
        SourceGrantId = sourceGrantId;
    }
}
