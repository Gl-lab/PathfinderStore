using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class HuntersEdgeTests
{
    [Fact]
    public void Constructor_CopiesEffects()
    {
        List<HuntersEdgeEffectDescriptor> effects =
        [
            new HuntersEdgeEffectDescriptor(
                "hunters_edge.flurry.effect.level_1",
                HuntersEdgeEffectKind.MultipleAttackPenalty,
                "Flurry",
                "Reduced multiple attack penalty." ),
        ];
        HuntersEdge huntersEdge = new HuntersEdge(
            "hunters_edge.flurry",
            "Flurry",
            SourceReference.Unknown,
            effects,
            [] );

        effects.Clear();

        Assert.Single( huntersEdge.Effects );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "edge.flurry" )]
    public void Constructor_InvalidId_Throws( string id )
    {
        Assert.Throws<ArgumentException>( () => new HuntersEdge(
            id,
            "Flurry",
            SourceReference.Unknown,
            [
                new HuntersEdgeEffectDescriptor(
                    "hunters_edge.flurry.effect.level_1",
                    HuntersEdgeEffectKind.MultipleAttackPenalty,
                    "Flurry",
                    "Summary" ),
            ],
            [] ) );
    }

    [Fact]
    public void EffectDescriptor_EmptySummary_Throws()
    {
        Assert.Throws<ArgumentException>( () => new HuntersEdgeEffectDescriptor(
            "hunters_edge.flurry.effect.level_1",
            HuntersEdgeEffectKind.MultipleAttackPenalty,
            "Flurry",
            " " ) );
    }

    [Fact]
    public void EffectDescriptor_UnknownKind_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>( () => new HuntersEdgeEffectDescriptor(
            "hunters_edge.flurry.effect.level_1",
            ( HuntersEdgeEffectKind )Int32.MaxValue,
            "Flurry",
            "Summary" ) );
    }
}
