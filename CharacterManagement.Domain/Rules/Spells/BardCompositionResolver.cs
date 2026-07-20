using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed record BardCompositionPackage(
    int MaximumFocusPoints,
    SpellDefinition CompositionCantrip,
    SpellDefinition FocusSpell,
    string SourceGrantId );

public static class BardCompositionResolver
{
    private const string CompositionCantripId = "spell.courageous_anthem";
    private const string FocusSpellId = "spell.counter_performance";
    private const string SourceGrantId = "class_feature.bard.composition_spells";

    public static BardCompositionPackage Resolve( IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( spellCatalog );

        IReadOnlyDictionary<string, SpellDefinition> definitions = spellCatalog
            .ToDictionary( spell => spell.Id, StringComparer.Ordinal );
        SpellDefinition compositionCantrip = ResolveSpell(
            definitions,
            CompositionCantripId,
            SpellKind.Cantrip );
        SpellDefinition focusSpell = ResolveSpell(
            definitions,
            FocusSpellId,
            SpellKind.Focus );

        return new BardCompositionPackage(
            1,
            compositionCantrip,
            focusSpell,
            SourceGrantId );
    }

    private static SpellDefinition ResolveSpell(
        IReadOnlyDictionary<string, SpellDefinition> definitions,
        string spellId,
        SpellKind expectedKind )
    {
        if ( !definitions.TryGetValue( spellId, out SpellDefinition? spell ) )
        {
            throw new ArgumentException(
                $"Bard composition spell '{spellId}' is not defined.",
                nameof( definitions ) );
        }

        if ( ( spell.Kind != expectedKind ) ||
             ( spell.Rank != 1 ) ||
             !spell.Traditions.Contains( SpellTradition.Occult ) )
        {
            throw new ArgumentException(
                $"Bard composition spell '{spellId}' has invalid metadata.",
                nameof( definitions ) );
        }

        return spell;
    }
}
