using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class DruidicOrderDtoMapper
{
    public static DruidicOrderDto Map( DruidicOrder druidicOrder )
    {
        ArgumentNullException.ThrowIfNull( druidicOrder );

        return new DruidicOrderDto
        {
            Id = druidicOrder.Id,
            Name = druidicOrder.Name,
            Source = new SourceReferenceDto
            {
                Book = druidicOrder.Source.Book,
                Page = druidicOrder.Source.Page,
            },
            SkillGrant = Map( druidicOrder.SkillGrant ),
            Benefits = druidicOrder.Benefits.Select( Map ).ToArray(),
        };
    }

    public static DruidicOrderPackageDto MapPackage( DruidicOrder druidicOrder )
    {
        ArgumentNullException.ThrowIfNull( druidicOrder );

        return new DruidicOrderPackageDto
        {
            Id = druidicOrder.Id,
            Name = druidicOrder.Name,
            SkillGrant = Map( druidicOrder.SkillGrant ),
            Benefits = druidicOrder.Benefits.Select( Map ).ToArray(),
        };
    }

    private static ClassSkillGrantDto Map( ClassSkillGrantDescriptor grant )
    {
        return new ClassSkillGrantDto
        {
            Id = grant.Id,
            SkillOptions = grant.SkillOptions.ToArray(),
        };
    }

    private static DruidicOrderBenefitDto Map( DruidicOrderBenefitDescriptor benefit )
    {
        return new DruidicOrderBenefitDto
        {
            Id = benefit.Id,
            Kind = benefit.Kind,
            Name = benefit.Name,
            DeferredDependencies = benefit.DeferredDependencies.ToArray(),
        };
    }
}
