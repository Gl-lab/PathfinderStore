using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class DruidicOrderTests
{
    [Fact]
    public void Constructor_ValidOrder_PreservesTypedBenefits()
    {
        DruidicOrder druidicOrder = CreateOrder();

        Assert.Equal( "druidic_order.animal.skill.order", druidicOrder.SkillGrant.Id );
        Assert.Contains(
            druidicOrder.Benefits,
            benefit => benefit.Kind == DruidicOrderBenefitKind.ClassFeat );
        Assert.Contains(
            druidicOrder.Benefits,
            benefit => benefit.Kind == DruidicOrderBenefitKind.FocusSpell );
    }

    [Fact]
    public void Constructor_DuplicateBenefitKind_Throws()
    {
        Assert.Throws<ArgumentException>( () => new DruidicOrder(
            "druidic_order.animal",
            "Animal",
            SourceReference.Unknown,
            new ClassSkillGrantDescriptor(
                "druidic_order.animal.skill.order",
                [ "skill.athletics" ] ),
            [
                CreateBenefit( "feat.animal_companion", DruidicOrderBenefitKind.ClassFeat ),
                CreateBenefit( "feat.animal_empathy", DruidicOrderBenefitKind.ClassFeat ),
            ] ) );
    }

    private static DruidicOrder CreateOrder()
    {
        return new DruidicOrder(
            "druidic_order.animal",
            "Animal",
            SourceReference.Unknown,
            new ClassSkillGrantDescriptor(
                "druidic_order.animal.skill.order",
                [ "skill.athletics" ] ),
            [
                CreateBenefit( "feat.animal_companion", DruidicOrderBenefitKind.ClassFeat ),
                CreateBenefit( "spell.heal_animal", DruidicOrderBenefitKind.FocusSpell ),
            ] );
    }

    private static DruidicOrderBenefitDescriptor CreateBenefit(
        string id,
        DruidicOrderBenefitKind kind )
    {
        return new DruidicOrderBenefitDescriptor(
            id,
            kind,
            id,
            [ CharacterClassDependencyType.ClassFeatureRules ] );
    }
}
