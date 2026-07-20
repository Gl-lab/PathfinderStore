using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class DruidFocusPoolResolverTests
{
    [Fact]
    public void Resolve_ReturnsOrderSpellAndOneFocusPoint()
    {
        DruidicOrder order = CreateOrder();
        IReadOnlyCollection<SpellDefinition> catalog =
        [
            Spell( "spell.heal_animal", SpellKind.Focus, SpellTradition.Primal ),
        ];

        DruidFocusPool result = DruidFocusPoolResolver.Resolve( order, catalog );

        Assert.Equal( 1, result.MaximumFocusPoints );
        Assert.Equal( "spell.heal_animal", result.FocusSpell.Id );
        Assert.Equal( order.Id, result.SourceGrantId );
    }

    [Fact]
    public void Resolve_RejectsInvalidFocusSpellMetadata()
    {
        IReadOnlyCollection<SpellDefinition> catalog =
        [
            Spell( "spell.heal_animal", SpellKind.Spell, SpellTradition.Primal ),
        ];

        Assert.Throws<ArgumentException>( () => DruidFocusPoolResolver.Resolve( CreateOrder(), catalog ) );
    }

    private static DruidicOrder CreateOrder()
    {
        return new DruidicOrder(
            "druidic_order.animal",
            "Animal",
            new SourceReference( "Player Core", 130 ),
            new ClassSkillGrantDescriptor(
                "druidic_order.animal.skill.order",
                [ "skill.athletics" ] ),
            [
                new DruidicOrderBenefitDescriptor(
                    "feat.animal_companion",
                    DruidicOrderBenefitKind.ClassFeat,
                    "Animal Companion",
                    [] ),
                new DruidicOrderBenefitDescriptor(
                    "spell.heal_animal",
                    DruidicOrderBenefitKind.FocusSpell,
                    "Heal Animal",
                    [] ),
            ] );
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
            SpellRarity.Uncommon,
            new SourceReference( "Player Core", 1 ) );
    }
}
