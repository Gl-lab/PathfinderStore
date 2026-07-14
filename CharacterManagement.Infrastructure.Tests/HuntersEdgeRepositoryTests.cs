using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class HuntersEdgeRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsThreePlayerCoreEdges()
    {
        HuntersEdgeRepository repository = new HuntersEdgeRepository();

        IReadOnlyCollection<HuntersEdge> huntersEdges = repository.GetAll();

        Assert.Equal( 3, huntersEdges.Count );
        Assert.Equal(
            [ "hunters_edge.flurry", "hunters_edge.outwit", "hunters_edge.precision" ],
            huntersEdges.Select( huntersEdge => huntersEdge.Id ).OrderBy( id => id ) );
        Assert.All( huntersEdges, huntersEdge =>
            Assert.Equal( "Player Core", huntersEdge.Source.Book ) );
    }

    [Fact]
    public void GetHuntersEdge_Precision_ReturnsTypedEffect()
    {
        HuntersEdgeRepository repository = new HuntersEdgeRepository();

        HuntersEdge huntersEdge = repository.GetHuntersEdge( "hunters_edge.precision" );

        Assert.Equal(
            HuntersEdgeEffectKind.PrecisionDamage,
            Assert.Single( huntersEdge.Effects ).Kind );
    }
}
