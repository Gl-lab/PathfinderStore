using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public static class BardSpellLoadoutResolver
{
    private const int CantripCount = 5;
    private const int SelectedRankOneSpellCount = 2;

    public static BardSpellLoadout Resolve(
        BardMuse bardMuse,
        IReadOnlyList<string> cantripIds,
        IReadOnlyList<string> selectedRankOneSpellIds,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( bardMuse );
        ArgumentNullException.ThrowIfNull( cantripIds );
        ArgumentNullException.ThrowIfNull( selectedRankOneSpellIds );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        ValidateExactUniqueCount( cantripIds, CantripCount, "cantrips" );
        ValidateExactUniqueCount(
            selectedRankOneSpellIds,
            SelectedRankOneSpellCount,
            "rank-1 repertoire spells" );

        IReadOnlySet<string> availableCantripIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, SpellTradition.Occult, 1, SpellKind.Cantrip )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> availableRankOneSpellIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, SpellTradition.Occult, 1, SpellKind.Spell )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );

        ValidateAvailability( cantripIds, availableCantripIds, "Bard cantrip" );
        ValidateAvailability(
            selectedRankOneSpellIds,
            availableRankOneSpellIds,
            "Bard rank-1 repertoire spell" );

        string museGrantedSpellId = bardMuse.Benefits
            .Single( benefit => benefit.Kind == BardMuseBenefitKind.RepertoireSpell )
            .Id;
        SpellDefinition museGrantedSpell = spellCatalog
            .SingleOrDefault( spell => spell.Id == museGrantedSpellId ) ??
            throw new ArgumentException(
                $"Muse-granted spell '{museGrantedSpellId}' is not defined in the spell catalog.",
                nameof( spellCatalog ) );

        if ( museGrantedSpell.Kind != SpellKind.Spell ||
             museGrantedSpell.Rank != 1 ||
             !museGrantedSpell.Traditions.Contains( SpellTradition.Occult ) )
        {
            throw new ArgumentException(
                $"Muse-granted spell '{museGrantedSpellId}' must be an occult rank-1 spell.",
                nameof( spellCatalog ) );
        }

        if ( selectedRankOneSpellIds.Contains( museGrantedSpellId, StringComparer.Ordinal ) )
        {
            throw new ArgumentException(
                "Selected Bard repertoire spells cannot duplicate the Muse-granted spell.",
                nameof( selectedRankOneSpellIds ) );
        }

        return new BardSpellLoadout(
            cantripIds,
            selectedRankOneSpellIds,
            museGrantedSpellId );
    }

    private static void ValidateExactUniqueCount(
        IReadOnlyList<string> spellIds,
        int expectedCount,
        string groupName )
    {
        if ( spellIds.Count != expectedCount )
        {
            throw new ArgumentException(
                $"A first-level Bard must select exactly {expectedCount} {groupName}.",
                nameof( spellIds ) );
        }

        if ( spellIds.Any( String.IsNullOrWhiteSpace ) ||
             spellIds.Distinct( StringComparer.Ordinal ).Count() != spellIds.Count )
        {
            throw new ArgumentException(
                $"Selected Bard {groupName} must be non-empty and unique.",
                nameof( spellIds ) );
        }
    }

    private static void ValidateAvailability(
        IReadOnlyList<string> spellIds,
        IReadOnlySet<string> availableSpellIds,
        string groupName )
    {
        string? unavailableSpellId = spellIds.FirstOrDefault( spellId => !availableSpellIds.Contains( spellId ) );
        if ( unavailableSpellId is not null )
        {
            throw new ArgumentException(
                $"{groupName} '{unavailableSpellId}' is not available.",
                nameof( spellIds ) );
        }
    }
}
