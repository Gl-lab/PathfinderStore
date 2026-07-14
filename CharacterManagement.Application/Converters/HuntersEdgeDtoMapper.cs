using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class HuntersEdgeDtoMapper
{
    public static HuntersEdgeDto Map( HuntersEdge huntersEdge )
    {
        ArgumentNullException.ThrowIfNull( huntersEdge );

        return new HuntersEdgeDto
        {
            Id = huntersEdge.Id,
            Name = huntersEdge.Name,
            Source = new SourceReferenceDto
            {
                Book = huntersEdge.Source.Book,
                Page = huntersEdge.Source.Page,
            },
            Effects = huntersEdge.Effects.Select( Map ).ToArray(),
            DeferredDependencies = huntersEdge.DeferredDependencies.ToArray(),
        };
    }

    public static HuntersEdgePackageDto MapPackage( HuntersEdge huntersEdge )
    {
        ArgumentNullException.ThrowIfNull( huntersEdge );

        return new HuntersEdgePackageDto
        {
            Id = huntersEdge.Id,
            Name = huntersEdge.Name,
            Effects = huntersEdge.Effects.Select( Map ).ToArray(),
        };
    }

    private static HuntersEdgeEffectDto Map( HuntersEdgeEffectDescriptor effect )
    {
        return new HuntersEdgeEffectDto
        {
            Id = effect.Id,
            Kind = effect.Kind,
            Name = effect.Name,
            Summary = effect.Summary,
        };
    }
}
