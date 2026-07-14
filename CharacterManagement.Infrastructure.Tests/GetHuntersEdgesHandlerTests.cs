using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.HuntersEdges;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetHuntersEdgesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalog()
    {
        GetHuntersEdgesHandler handler = new GetHuntersEdgesHandler( new HuntersEdgeRepository() );

        IReadOnlyCollection<HuntersEdgeDto> result = await handler.Handle(
            new GetHuntersEdgesCommand(),
            CancellationToken.None );

        Assert.Equal( [ "Flurry", "Outwit", "Precision" ], result.Select( item => item.Name ) );
        HuntersEdgeDto precision = Assert.Single(
            result.Where( item => item.Id == "hunters_edge.precision" ) );
        Assert.Equal( HuntersEdgeEffectKind.PrecisionDamage, Assert.Single( precision.Effects ).Kind );
    }
}
