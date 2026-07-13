using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class ClericDoctrineDtoMapper
{
    public static ClericDoctrineDto Map( ClericDoctrine doctrine )
    {
        ArgumentNullException.ThrowIfNull( doctrine );

        return new ClericDoctrineDto
        {
            Id = doctrine.Id,
            Name = doctrine.Name,
            Source = new SourceReferenceDto
            {
                Book = doctrine.Source.Book,
                Page = doctrine.Source.Page,
            },
            ProficiencyGrants = CharacterClassDtoMapper.MapProficiencies( doctrine.ProficiencyGrants ),
            Effects = doctrine.Effects
                .Select( Map )
                .ToArray(),
            DeferredDependencies = doctrine.DeferredDependencies,
        };
    }

    public static ClericDoctrinePackageDto MapPackage( ClericDoctrine doctrine )
    {
        ArgumentNullException.ThrowIfNull( doctrine );

        return new ClericDoctrinePackageDto
        {
            Id = doctrine.Id,
            Name = doctrine.Name,
            Effects = doctrine.Effects
                .Select( Map )
                .ToArray(),
            DeferredDependencies = doctrine.DeferredDependencies,
        };
    }

    private static ClericDoctrineEffectDto Map( ClericDoctrineEffectDescriptor effect )
    {
        return new ClericDoctrineEffectDto
        {
            Id = effect.Id,
            Name = effect.Name,
            Summary = effect.Summary,
            DeferredDependencies = effect.DeferredDependencies,
        };
    }
}
