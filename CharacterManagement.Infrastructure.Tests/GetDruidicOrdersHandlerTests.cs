using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.DruidicOrders;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetDruidicOrdersHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalog()
    {
        GetDruidicOrdersHandler handler = new GetDruidicOrdersHandler(
            new DruidicOrderRepository() );

        IReadOnlyCollection<DruidicOrderDto> result = await handler.Handle(
            new GetDruidicOrdersCommand(),
            CancellationToken.None );

        Assert.Equal( [ "Animal", "Leaf", "Storm", "Untamed" ], result.Select( item => item.Name ) );
        DruidicOrderDto animal = Assert.Single(
            result.Where( item => item.Id == "druidic_order.animal" ) );
        Assert.Equal( "skill.athletics", Assert.Single( animal.SkillGrant.SkillOptions ) );
        Assert.Equal( 2, animal.Benefits.Count );
    }
}
