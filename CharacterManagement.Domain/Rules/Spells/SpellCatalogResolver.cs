using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public static class SpellCatalogResolver
{
    public static IReadOnlyList<SpellDefinition> ResolveCommonOptions(
        IReadOnlyCollection<SpellDefinition> catalog,
        SpellTradition tradition,
        int rank,
        SpellKind kind )
    {
        ArgumentNullException.ThrowIfNull( catalog );

        if ( rank < 1 )
        {
            throw new ArgumentOutOfRangeException( nameof( rank ), "Spell rank must be positive." );
        }

        if ( kind == SpellKind.Focus )
        {
            throw new ArgumentException(
                "Focus spells require an explicit class feature source.",
                nameof( kind ) );
        }

        return catalog
            .Where( spell => spell.Rarity == SpellRarity.Common )
            .Where( spell => spell.Rank == rank )
            .Where( spell => spell.Kind == kind )
            .Where( spell => spell.Traditions.Contains( tradition ) )
            .OrderBy( spell => spell.Name, StringComparer.Ordinal )
            .ToArray();
    }
}
