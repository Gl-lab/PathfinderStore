using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public enum ClericSpellAccessSource
{
    DivineTradition,
    DeityGranted
}

public sealed record ClericAvailableSpell(
    SpellDefinition Spell,
    SpellTradition EffectiveTradition,
    IReadOnlyList<ClericSpellAccessSource> AccessSources );

public static class ClericSpellAvailabilityResolver
{
    public static IReadOnlyList<ClericAvailableSpell> ResolveCantrips(
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( spellCatalog );

        return spellCatalog
            .Where( spell =>
                spell.Kind == SpellKind.Cantrip &&
                spell.Rank == 1 &&
                spell.Rarity == SpellRarity.Common &&
                spell.Traditions.Contains( SpellTradition.Divine ) )
            .OrderBy( spell => spell.Name, StringComparer.Ordinal )
            .Select( spell => new ClericAvailableSpell(
                spell,
                SpellTradition.Divine,
                [ ClericSpellAccessSource.DivineTradition ] ) )
            .ToArray();
    }

    public static IReadOnlyList<ClericAvailableSpell> ResolveRankOneSpells(
        Deity deity,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( deity );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        HashSet<string> grantedSpellIds = deity.GrantedSpells
            .Where( spell => spell.Rank == 1 )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );

        return spellCatalog
            .Where( spell => spell.Kind == SpellKind.Spell && spell.Rank == 1 )
            .Select( spell => CreateAvailability( spell, grantedSpellIds ) )
            .Where( spell => spell is not null )
            .Cast<ClericAvailableSpell>()
            .OrderBy( spell => spell.Spell.Name, StringComparer.Ordinal )
            .ToArray();
    }

    private static ClericAvailableSpell? CreateAvailability(
        SpellDefinition spell,
        IReadOnlySet<string> grantedSpellIds )
    {
        bool isDivineListSpell = spell.Rarity == SpellRarity.Common &&
                                 spell.Traditions.Contains( SpellTradition.Divine );
        bool isDeityGranted = grantedSpellIds.Contains( spell.Id );
        if ( !isDivineListSpell && !isDeityGranted )
        {
            return null;
        }

        List<ClericSpellAccessSource> accessSources = [];
        if ( isDivineListSpell )
        {
            accessSources.Add( ClericSpellAccessSource.DivineTradition );
        }

        if ( isDeityGranted )
        {
            accessSources.Add( ClericSpellAccessSource.DeityGranted );
        }

        return new ClericAvailableSpell( spell, SpellTradition.Divine, accessSources );
    }
}
