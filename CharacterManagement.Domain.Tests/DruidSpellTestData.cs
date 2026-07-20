using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

internal static class DruidSpellTestData
{
    public static DruidSpellLoadout CreateLoadout()
    {
        string[] cantripIds =
            [ "spell.cantrip_1", "spell.cantrip_2", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ];
        SourceReference source = new SourceReference( "Player Core", 1 );
        IReadOnlyCollection<SpellDefinition> catalog =
        [
            .. cantripIds.Select( ( id, index ) => new SpellDefinition(
                id,
                $"Cantrip {index + 1}",
                1,
                SpellKind.Cantrip,
                [ SpellTradition.Primal ],
                [],
                SpellRarity.Common,
                source ) ),
            new SpellDefinition(
                "spell.heal",
                "Heal",
                1,
                SpellKind.Spell,
                [ SpellTradition.Primal ],
                [],
                SpellRarity.Common,
                source ),
        ];
        return DruidSpellLoadoutResolver.Resolve(
            cantripIds,
            [ "spell.heal", "spell.heal" ],
            catalog );
    }
}
