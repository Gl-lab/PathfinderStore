using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed record DruidFocusPool( int MaximumFocusPoints, SpellDefinition FocusSpell, string SourceGrantId );

public static class DruidFocusPoolResolver
{
    public static DruidFocusPool Resolve(
        DruidicOrder druidicOrder,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( druidicOrder );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        string focusSpellId = druidicOrder.Benefits
            .Single( benefit => benefit.Kind == DruidicOrderBenefitKind.FocusSpell )
            .Id;
        SpellDefinition focusSpell = spellCatalog
            .SingleOrDefault( spell => spell.Id == focusSpellId ) ??
            throw new ArgumentException(
                $"Druid Order spell '{focusSpellId}' is not defined.",
                nameof( spellCatalog ) );
        if ( focusSpell.Kind != SpellKind.Focus ||
             focusSpell.Rank != 1 ||
             !focusSpell.Traditions.Contains( SpellTradition.Primal ) )
        {
            throw new ArgumentException(
                $"Druid Order spell '{focusSpellId}' has invalid metadata.",
                nameof( spellCatalog ) );
        }

        return new DruidFocusPool( 1, focusSpell, druidicOrder.Id );
    }
}
