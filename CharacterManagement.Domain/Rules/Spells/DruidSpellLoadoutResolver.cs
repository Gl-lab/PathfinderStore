using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public static class DruidSpellLoadoutResolver
{
    public static DruidSpellLoadout Resolve(
        IReadOnlyList<string> cantripIds,
        IReadOnlyList<string> preparedSpellIds,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( cantripIds );
        ArgumentNullException.ThrowIfNull( preparedSpellIds );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        if ( cantripIds.Count != 5 ||
             cantripIds.Distinct( StringComparer.Ordinal ).Count() != 5 )
        {
            throw new ArgumentException(
                "A first-level Druid must prepare exactly 5 unique cantrips.",
                nameof( cantripIds ) );
        }

        if ( preparedSpellIds.Count != 2 || preparedSpellIds.Any( String.IsNullOrWhiteSpace ) )
        {
            throw new ArgumentException(
                "A first-level Druid must prepare exactly 2 rank-1 spell slots.",
                nameof( preparedSpellIds ) );
        }

        IReadOnlySet<string> availableCantripIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, SpellTradition.Primal, 1, SpellKind.Cantrip )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> availableSpellIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, SpellTradition.Primal, 1, SpellKind.Spell )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );

        if ( cantripIds.Any( spellId => !availableCantripIds.Contains( spellId ) ) )
        {
            throw new ArgumentException( "Druid cantrips must come from the common Primal list.", nameof( cantripIds ) );
        }

        if ( preparedSpellIds.Any( spellId => !availableSpellIds.Contains( spellId ) ) )
        {
            throw new ArgumentException(
                "Prepared Druid spells must come from the common Primal rank-1 list.",
                nameof( preparedSpellIds ) );
        }

        return new DruidSpellLoadout( cantripIds.ToArray(), preparedSpellIds.ToArray() );
    }
}
