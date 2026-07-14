using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public static class ClericSpellLoadoutResolver
{
    private const int CantripCount = 5;
    private const int PreparedSpellCount = 2;

    public static ClericSpellLoadout Resolve(
        Deity deity,
        IReadOnlyList<string> cantripIds,
        IReadOnlyList<string> preparedSpellIds,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( deity );
        ArgumentNullException.ThrowIfNull( cantripIds );
        ArgumentNullException.ThrowIfNull( preparedSpellIds );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        if ( cantripIds.Count != CantripCount )
        {
            throw new CharacterManagementException(
                $"A first-level Cleric must prepare exactly {CantripCount} cantrips." );
        }

        if ( cantripIds.Distinct( StringComparer.Ordinal ).Count() != CantripCount )
        {
            throw new CharacterManagementException( "Prepared Cleric cantrips must be unique." );
        }

        if ( preparedSpellIds.Count != PreparedSpellCount )
        {
            throw new CharacterManagementException(
                $"A first-level Cleric must prepare exactly {PreparedSpellCount} rank-1 spell slots." );
        }

        IReadOnlySet<string> availableCantripIds = ClericSpellAvailabilityResolver
            .ResolveCantrips( spellCatalog )
            .Select( spell => spell.Spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> availableSpellIds = ClericSpellAvailabilityResolver
            .ResolveRankOneSpells( deity, spellCatalog )
            .Select( spell => spell.Spell.Id )
            .ToHashSet( StringComparer.Ordinal );

        string? invalidCantripId = cantripIds.FirstOrDefault(
            spellId => !availableCantripIds.Contains( spellId ) );
        if ( invalidCantripId is not null )
        {
            throw new CharacterManagementException(
                $"Cantrip '{invalidCantripId}' is not available to a first-level Cleric." );
        }

        string? invalidSpellId = preparedSpellIds.FirstOrDefault(
            spellId => !availableSpellIds.Contains( spellId ) );
        if ( invalidSpellId is not null )
        {
            throw new CharacterManagementException(
                $"Spell '{invalidSpellId}' is not available to Cleric of deity '{deity.Id}'." );
        }

        return new ClericSpellLoadout( cantripIds, preparedSpellIds );
    }
}
