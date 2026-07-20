using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public static class WitchSpellLoadoutResolver
{
    private static readonly IReadOnlySet<string> _focusHexIds = new HashSet<string>(
        [ "spell.patron_s_puppet", "spell.phase_familiar" ],
        StringComparer.Ordinal );

    public static WitchSpellLoadout Resolve(
        WitchPatron patron,
        string? patronFamiliarSpellId,
        IReadOnlyList<string> familiarCantripIds,
        IReadOnlyList<string> familiarRankOneSpellIds,
        IReadOnlyList<string> preparedCantripIds,
        IReadOnlyList<string> preparedSpellIds,
        string focusHexId,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( patron );
        ArgumentNullException.ThrowIfNull( familiarCantripIds );
        ArgumentNullException.ThrowIfNull( familiarRankOneSpellIds );
        ArgumentNullException.ThrowIfNull( preparedCantripIds );
        ArgumentNullException.ThrowIfNull( preparedSpellIds );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        ValidateUniqueCount( familiarCantripIds, 10, "A Witch familiar must know exactly 10 unique cantrips." );
        ValidateUniqueCount( familiarRankOneSpellIds, 5, "A Witch familiar must know exactly 5 unique rank-1 spells." );
        ValidateUniqueCount( preparedCantripIds, 5, "A first-level Witch must prepare exactly 5 unique cantrips." );
        if ( ( preparedSpellIds.Count != 2 ) || preparedSpellIds.Any( String.IsNullOrWhiteSpace ) )
        {
            throw new ArgumentException( "A first-level Witch must prepare exactly 2 rank-1 spell slots." );
        }

        IReadOnlySet<string> availableCantripIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, patron.SpellTradition, 1, SpellKind.Cantrip )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> availableSpellIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, patron.SpellTradition, 1, SpellKind.Spell )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );

        if ( familiarCantripIds.Any( spellId => !availableCantripIds.Contains( spellId ) ) )
        {
            throw new ArgumentException( "Witch familiar cantrips must match the Patron tradition." );
        }

        WitchPatronBenefitDescriptor patronSpellBenefit = patron.ResolveFamiliarSpell( patronFamiliarSpellId );
        SpellDefinition patronSpell = ResolveSpell( patronSpellBenefit.Id, spellCatalog );
        if ( ( patronSpell.Kind != SpellKind.Spell ) ||
             ( patronSpell.Rank != 1 ) ||
             !patronSpell.Traditions.Contains( patron.SpellTradition ) )
        {
            throw new ArgumentException( "Patron-granted familiar spell has invalid metadata." );
        }

        if ( familiarRankOneSpellIds.Any( spellId => !availableSpellIds.Contains( spellId ) ) ||
             familiarRankOneSpellIds.Contains( patronSpell.Id, StringComparer.Ordinal ) )
        {
            throw new ArgumentException( "Witch familiar spells must be common Patron-tradition spells distinct from the Patron grant." );
        }

        IReadOnlySet<string> knownCantripIds = familiarCantripIds.ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> knownSpellIds = familiarRankOneSpellIds
            .Append( patronSpell.Id )
            .ToHashSet( StringComparer.Ordinal );
        if ( preparedCantripIds.Any( spellId => !knownCantripIds.Contains( spellId ) ) ||
             preparedSpellIds.Any( spellId => !knownSpellIds.Contains( spellId ) ) )
        {
            throw new ArgumentException( "Prepared Witch spells must be known by the familiar." );
        }

        if ( !_focusHexIds.Contains( focusHexId ) )
        {
            throw new ArgumentException( "Witch focus hex must be Patron's Puppet or Phase Familiar.", nameof( focusHexId ) );
        }

        SpellDefinition focusHex = ResolveSpell( focusHexId, spellCatalog );
        if ( ( focusHex.Kind != SpellKind.Focus ) || !focusHex.Traditions.Contains( patron.SpellTradition ) )
        {
            throw new ArgumentException( "Witch focus hex has invalid metadata.", nameof( spellCatalog ) );
        }

        return new WitchSpellLoadout(
            familiarCantripIds,
            familiarRankOneSpellIds,
            patronSpell.Id,
            preparedCantripIds,
            preparedSpellIds,
            focusHexId );
    }

    private static void ValidateUniqueCount( IReadOnlyList<string> spellIds, int count, string message )
    {
        if ( ( spellIds.Count != count ) ||
             ( spellIds.Distinct( StringComparer.Ordinal ).Count() != count ) )
        {
            throw new ArgumentException( message );
        }
    }

    private static SpellDefinition ResolveSpell(
        string spellId,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        return spellCatalog.SingleOrDefault( spell => spell.Id == spellId ) ??
               throw new ArgumentException( $"Spell '{spellId}' is not defined.", nameof( spellCatalog ) );
    }
}
