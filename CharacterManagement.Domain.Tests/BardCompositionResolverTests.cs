using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class BardCompositionResolverTests
{
    [Fact]
    public void Resolve_ReturnsInitialCompositionSpellsAndFocusPool()
    {
        IReadOnlyCollection<SpellDefinition> catalog =
        [
            Spell( "spell.courageous_anthem", "Courageous Anthem", SpellKind.Cantrip ),
            Spell( "spell.counter_performance", "Counter Performance", SpellKind.Focus ),
        ];

        BardCompositionPackage result = BardCompositionResolver.Resolve( catalog );

        Assert.Equal( 1, result.MaximumFocusPoints );
        Assert.Equal( "spell.courageous_anthem", result.CompositionCantrip.Id );
        Assert.Equal( "spell.counter_performance", result.FocusSpell.Id );
        Assert.Equal( "class_feature.bard.composition_spells", result.SourceGrantId );
    }

    [Fact]
    public void Resolve_RejectsInvalidCompositionMetadata()
    {
        IReadOnlyCollection<SpellDefinition> catalog =
        [
            Spell( "spell.courageous_anthem", "Courageous Anthem", SpellKind.Focus ),
            Spell( "spell.counter_performance", "Counter Performance", SpellKind.Focus ),
        ];

        Assert.Throws<ArgumentException>( () => BardCompositionResolver.Resolve( catalog ) );
    }

    private static SpellDefinition Spell( string id, string name, SpellKind kind )
    {
        return new SpellDefinition(
            id,
            name,
            1,
            kind,
            [ SpellTradition.Occult ],
            [],
            SpellRarity.Uncommon,
            new SourceReference( "Player Core", 370 ) );
    }
}
