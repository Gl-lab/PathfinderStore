using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class HuntersEdgeRepository : IHuntersEdgeRepository
{
    private static readonly Dictionary<string, HuntersEdge> HuntersEdges = CreateHuntersEdges()
        .ToDictionary( huntersEdge => huntersEdge.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<HuntersEdge> GetAll() => HuntersEdges.Values.ToList();

    public HuntersEdge GetHuntersEdge( string huntersEdgeId )
    {
        if ( String.IsNullOrWhiteSpace( huntersEdgeId ) )
        {
            throw new ArgumentException( "Hunter's Edge id cannot be empty.", nameof( huntersEdgeId ) );
        }

        if ( !HuntersEdges.TryGetValue( huntersEdgeId, out HuntersEdge? huntersEdge ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( huntersEdgeId ),
                $"Hunter's Edge '{huntersEdgeId}' is not defined." );
        }

        return huntersEdge;
    }

    private static IReadOnlyCollection<HuntersEdge> CreateHuntersEdges()
    {
        return
        [
            Create(
                "flurry",
                "Flurry",
                HuntersEdgeEffectKind.MultipleAttackPenalty,
                "Reduced multiple attack penalty against hunted prey." ),
            Create(
                "outwit",
                "Outwit",
                HuntersEdgeEffectKind.ConditionalBonuses,
                "Conditional skill and defense bonuses against hunted prey." ),
            Create(
                "precision",
                "Precision",
                HuntersEdgeEffectKind.PrecisionDamage,
                "Additional precision damage on the first hit against hunted prey each round." ),
        ];
    }

    private static HuntersEdge Create(
        string id,
        string name,
        HuntersEdgeEffectKind effectKind,
        string summary )
    {
        string huntersEdgeId = $"hunters_edge.{id}";
        return new HuntersEdge(
            huntersEdgeId,
            name,
            new SourceReference( "Player Core", 154 ),
            [
                new HuntersEdgeEffectDescriptor(
                    $"{huntersEdgeId}.effect.level_1",
                    effectKind,
                    name,
                    summary ),
            ],
            [ CharacterClassDependencyType.ClassFeatureRules ] );
    }
}
