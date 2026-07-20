using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class DruidSpellLoadoutResolverTests
{
    [Fact]
    public void Resolve_AllowsPreparingSameSpellInBothSlots()
    {
        IReadOnlyCollection<SpellDefinition> catalog = CreateCatalog();

        DruidSpellLoadout result = DruidSpellLoadoutResolver.Resolve(
            [ "spell.cantrip_1", "spell.cantrip_2", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ],
            [ "spell.heal", "spell.heal" ],
            catalog );

        Assert.Equal( 5, result.CantripIds.Count );
        Assert.Equal( [ "spell.heal", "spell.heal" ], result.PreparedSpellIds );
    }

    [Fact]
    public void Resolve_RejectsDuplicateCantrips()
    {
        Assert.Throws<ArgumentException>( () => DruidSpellLoadoutResolver.Resolve(
            [ "spell.cantrip_1", "spell.cantrip_1", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ],
            [ "spell.heal", "spell.fear" ],
            CreateCatalog() ) );
    }

    [Fact]
    public void Resolve_RejectsSpellOutsideCommonPrimalList()
    {
        Assert.Throws<ArgumentException>( () => DruidSpellLoadoutResolver.Resolve(
            [ "spell.cantrip_1", "spell.cantrip_2", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ],
            [ "spell.heal", "spell.arcane" ],
            CreateCatalog() ) );
    }

    private static IReadOnlyCollection<SpellDefinition> CreateCatalog()
    {
        List<SpellDefinition> spells = Enumerable
            .Range( 1, 5 )
            .Select( index => Spell( $"spell.cantrip_{index}", SpellKind.Cantrip, SpellTradition.Primal ) )
            .ToList();
        spells.Add( Spell( "spell.heal", SpellKind.Spell, SpellTradition.Primal ) );
        spells.Add( Spell( "spell.fear", SpellKind.Spell, SpellTradition.Primal ) );
        spells.Add( Spell( "spell.arcane", SpellKind.Spell, SpellTradition.Arcane ) );
        return spells;
    }

    private static SpellDefinition Spell( string id, SpellKind kind, SpellTradition tradition )
    {
        return new SpellDefinition(
            id,
            id,
            1,
            kind,
            [ tradition ],
            [],
            SpellRarity.Common,
            new SourceReference( "Player Core", 1 ) );
    }
}
