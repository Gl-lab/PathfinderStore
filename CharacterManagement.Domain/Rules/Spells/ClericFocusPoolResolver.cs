using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public static class ClericFocusPoolResolver
{
    public const string DomainInitiateSourceGrantId =
        "cleric_doctrine.cloistered.effect.domain_initiate";

    public static ClericFocusPool Resolve(
        ClericDomain domain,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( domain );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        SpellDefinition? focusSpell = spellCatalog.SingleOrDefault(
            spell => spell.Id == domain.InitialFocusSpell.Id );
        if ( focusSpell is null )
        {
            throw new CharacterManagementException(
                $"Initial focus spell '{domain.InitialFocusSpell.Id}' is missing from the spell catalog." );
        }

        if ( focusSpell.Kind != SpellKind.Focus || focusSpell.Rank != 1 )
        {
            throw new CharacterManagementException(
                $"Initial domain spell '{focusSpell.Id}' must be a rank-1 focus spell." );
        }

        return new ClericFocusPool( 1, focusSpell, DomainInitiateSourceGrantId );
    }
}
