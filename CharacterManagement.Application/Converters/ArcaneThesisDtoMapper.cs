using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class ArcaneThesisDtoMapper
{
    public static ArcaneThesisDto Map( ArcaneThesis thesis )
    {
        ArgumentNullException.ThrowIfNull( thesis );

        return new ArcaneThesisDto
        {
            Id = thesis.Id,
            Name = thesis.Name,
            Source = new SourceReferenceDto
            {
                Book = thesis.Source.Book,
                Page = thesis.Source.Page,
            },
            Effects = thesis.Effects.Select( Map ).ToArray(),
        };
    }

    public static ArcaneThesisPackageDto MapPackage( ArcaneThesis thesis )
    {
        ArcaneThesisDto catalog = Map( thesis );
        return new ArcaneThesisPackageDto
        {
            Id = catalog.Id,
            Name = catalog.Name,
            Effects = catalog.Effects,
        };
    }

    private static ArcaneThesisEffectDto Map( ArcaneThesisEffectDescriptor effect )
    {
        return new ArcaneThesisEffectDto
        {
            Id = effect.Id,
            Kind = effect.Kind,
            Name = effect.Name,
            Summary = effect.Summary,
            MilestoneLevels = effect.MilestoneLevels.ToArray(),
            DeferredDependencies = effect.DeferredDependencies.ToArray(),
        };
    }
}
