using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class ArcaneSchoolDtoMapper
{
    public static ArcaneSchoolDto Map( ArcaneSchool school )
    {
        ArgumentNullException.ThrowIfNull( school );

        return new ArcaneSchoolDto
        {
            Id = school.Id,
            Name = school.Name,
            Source = new SourceReferenceDto
            {
                Book = school.Source.Book,
                Page = school.Source.Page,
            },
            HasCurriculum = school.HasCurriculum,
            CurriculumSpells = school.CurriculumSpells.Select( Map ).ToArray(),
            Benefits = school.Benefits.Select( Map ).ToArray(),
        };
    }

    public static ArcaneSchoolPackageDto MapPackage( ArcaneSchool school )
    {
        ArcaneSchoolDto catalog = Map( school );
        return new ArcaneSchoolPackageDto
        {
            Id = catalog.Id,
            Name = catalog.Name,
            HasCurriculum = catalog.HasCurriculum,
            CurriculumSpells = catalog.CurriculumSpells,
            Benefits = catalog.Benefits,
        };
    }

    private static ArcaneSchoolCurriculumSpellDto Map(
        ArcaneSchoolCurriculumSpellDescriptor spell )
    {
        return new ArcaneSchoolCurriculumSpellDto
        {
            Id = spell.Id,
            Name = spell.Name,
            Rank = spell.Rank,
            IsUncommon = spell.IsUncommon,
        };
    }

    private static ArcaneSchoolBenefitDto Map( ArcaneSchoolBenefitDescriptor benefit )
    {
        return new ArcaneSchoolBenefitDto
        {
            Id = benefit.Id,
            Kind = benefit.Kind,
            Name = benefit.Name,
            Summary = benefit.Summary,
            DeferredDependencies = benefit.DeferredDependencies.ToArray(),
        };
    }
}
