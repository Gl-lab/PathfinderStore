using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public sealed record WitchHexPackage(
    int MaximumFocusPoints,
    SpellDefinition PatronHexCantrip,
    SpellDefinition FocusHex,
    string SourceGrantId );

public static class WitchHexPackageResolver
{
    public static WitchHexPackage Resolve(
        WitchPatron patron,
        string focusHexId,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( patron );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        string patronHexId = patron.Benefits
            .Single( benefit => benefit.Kind == WitchPatronBenefitKind.HexCantrip )
            .Id;
        SpellDefinition patronHex = spellCatalog.Single( spell => spell.Id == patronHexId );
        SpellDefinition focusHex = spellCatalog.Single( spell => spell.Id == focusHexId );
        bool isInitialFocusHex = ( focusHexId == "spell.patron_s_puppet" ) ||
                                 ( focusHexId == "spell.phase_familiar" );
        if ( !isInitialFocusHex ||
             ( patronHex.Kind != SpellKind.Cantrip ) ||
             ( patronHex.Rank != 1 ) ||
             ( focusHex.Kind != SpellKind.Focus ) ||
             ( focusHex.Rank != 1 ) ||
             !patronHex.Traditions.Contains( patron.SpellTradition ) ||
             !focusHex.Traditions.Contains( patron.SpellTradition ) )
        {
            throw new ArgumentException( "Witch hex spell metadata is invalid.", nameof( spellCatalog ) );
        }

        return new WitchHexPackage( 1, patronHex, focusHex, patron.Id );
    }
}
