using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class SpellCatalogResolverTests
{
    [Fact]
    public void ResolveCommonOptions_ReturnsOnlyMatchingOrderedSpells()
    {
        IReadOnlyCollection<SpellDefinition> catalog =
        [
            Spell( "spell.zeta", "Zeta", SpellKind.Spell, SpellRarity.Common, SpellTradition.Occult ),
            Spell( "spell.alpha", "Alpha", SpellKind.Spell, SpellRarity.Common, SpellTradition.Occult ),
            Spell( "spell.uncommon", "Uncommon", SpellKind.Spell, SpellRarity.Uncommon, SpellTradition.Occult ),
            Spell( "spell.cantrip", "Cantrip", SpellKind.Cantrip, SpellRarity.Common, SpellTradition.Occult ),
            Spell( "spell.arcane", "Arcane", SpellKind.Spell, SpellRarity.Common, SpellTradition.Arcane ),
        ];

        IReadOnlyList<SpellDefinition> result = SpellCatalogResolver.ResolveCommonOptions(
            catalog,
            SpellTradition.Occult,
            1,
            SpellKind.Spell );

        Assert.Equal( [ "spell.alpha", "spell.zeta" ], result.Select( spell => spell.Id ) );
    }

    [Fact]
    public void ResolveCommonOptions_RejectsFocusSpellEnumeration()
    {
        Assert.Throws<ArgumentException>( () => SpellCatalogResolver.ResolveCommonOptions(
            [],
            SpellTradition.Primal,
            1,
            SpellKind.Focus ) );
    }

    private static SpellDefinition Spell(
        string id,
        string name,
        SpellKind kind,
        SpellRarity rarity,
        SpellTradition tradition )
    {
        return new SpellDefinition(
            id,
            name,
            1,
            kind,
            [ tradition ],
            [],
            rarity,
            new SourceReference( "Player Core", 1 ) );
    }
}
