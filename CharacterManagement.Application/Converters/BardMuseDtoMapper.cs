using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class BardMuseDtoMapper
{
    public static BardMuseDto Map( BardMuse bardMuse )
    {
        ArgumentNullException.ThrowIfNull( bardMuse );

        return new BardMuseDto
        {
            Id = bardMuse.Id,
            Name = bardMuse.Name,
            Source = new SourceReferenceDto
            {
                Book = bardMuse.Source.Book,
                Page = bardMuse.Source.Page,
            },
            Benefits = bardMuse.Benefits.Select( Map ).ToArray(),
        };
    }

    public static BardMusePackageDto MapPackage( BardMuse bardMuse )
    {
        ArgumentNullException.ThrowIfNull( bardMuse );

        return new BardMusePackageDto
        {
            Id = bardMuse.Id,
            Name = bardMuse.Name,
            Benefits = bardMuse.Benefits.Select( Map ).ToArray(),
        };
    }

    private static BardMuseBenefitDto Map( BardMuseBenefitDescriptor benefit )
    {
        return new BardMuseBenefitDto
        {
            Id = benefit.Id,
            Kind = benefit.Kind,
            Name = benefit.Name,
            DeferredDependencies = benefit.DeferredDependencies.ToArray(),
        };
    }
}
