using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class DruidicOrderRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsFourPlayerCoreOrders()
    {
        DruidicOrderRepository repository = new DruidicOrderRepository();

        IReadOnlyCollection<DruidicOrder> orders = repository.GetAll();

        Assert.Equal( 4, orders.Count );
        Assert.Equal(
            [
                "druidic_order.animal",
                "druidic_order.leaf",
                "druidic_order.storm",
                "druidic_order.untamed",
            ],
            orders.Select( order => order.Id ).OrderBy( id => id ) );
    }

    [Fact]
    public void GetDruidicOrder_Untamed_ReturnsVerifiedGrants()
    {
        DruidicOrderRepository repository = new DruidicOrderRepository();

        DruidicOrder order = repository.GetDruidicOrder( "druidic_order.untamed" );

        Assert.Equal( "skill.intimidation", Assert.Single( order.SkillGrant.SkillOptions ) );
        Assert.Contains( order.Benefits, benefit => benefit.Id == "feat.untamed_form" );
        Assert.Contains( order.Benefits, benefit => benefit.Id == "spell.untamed_shift" );
        DruidicOrderBenefitDescriptor focusSpell = Assert.Single(
            order.Benefits,
            benefit => benefit.Kind == DruidicOrderBenefitKind.FocusSpell );
        Assert.Empty( focusSpell.DeferredDependencies );
        Assert.Equal( 126, order.Source.Page );
    }
}
