using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

internal static class WitchSpellTestData
{
    public static WitchSpellLoadout CreateLoadout(
        WitchPatron patron,
        string? familiarSpellId = null )
    {
        string[] cantripIds = Enumerable.Range( 1, 10 ).Select( index => $"spell.cantrip_{index}" ).ToArray();
        string[] spellIds = Enumerable.Range( 1, 5 ).Select( index => $"spell.known_{index}" ).ToArray();
        SourceReference source = new SourceReference( "Player Core", 1 );
        List<SpellDefinition> catalog = cantripIds
            .Select( id => Spell( id, SpellKind.Cantrip, SpellRarity.Common, source ) )
            .Concat( spellIds.Select( id => Spell( id, SpellKind.Spell, SpellRarity.Common, source ) ) )
            .ToList();
        catalog.AddRange( patron.FamiliarSpellOptions.Select( benefit =>
            Spell( benefit.Id, SpellKind.Spell, SpellRarity.Common, source ) ) );
        catalog.Add( Spell( "spell.patron_s_puppet", SpellKind.Focus, SpellRarity.Uncommon, source ) );

        return WitchSpellLoadoutResolver.Resolve(
            patron,
            familiarSpellId,
            cantripIds,
            spellIds,
            cantripIds.Take( 5 ).ToArray(),
            [ spellIds[ 0 ], spellIds[ 0 ] ],
            "spell.patron_s_puppet",
            catalog );
    }

    private static SpellDefinition Spell(
        string id,
        SpellKind kind,
        SpellRarity rarity,
        SourceReference source )
    {
        return new SpellDefinition(
            id,
            id,
            1,
            kind,
            [ SpellTradition.Divine ],
            [],
            rarity,
            source );
    }
}
