using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class WitchPatronDtoMapper
{
    public static WitchPatronDto Map( WitchPatron patron )
    {
        ArgumentNullException.ThrowIfNull( patron );

        return new WitchPatronDto
        {
            Id = patron.Id,
            Name = patron.Name,
            Source = new SourceReferenceDto
            {
                Book = patron.Source.Book,
                Page = patron.Source.Page,
            },
            SpellTradition = patron.SpellTradition,
            SkillGrant = Map( patron.SkillGrant ),
            Benefits = patron.Benefits.Select( Map ).ToArray(),
        };
    }

    public static WitchPatronPackageDto MapPackage(
        WitchPatron patron,
        string selectedFamiliarSpellId )
    {
        ArgumentNullException.ThrowIfNull( patron );

        WitchPatronBenefitDescriptor selectedFamiliarSpell = patron.FamiliarSpellOptions
            .Single( benefit => benefit.Id == selectedFamiliarSpellId );
        return new WitchPatronPackageDto
        {
            Id = patron.Id,
            Name = patron.Name,
            SpellTradition = patron.SpellTradition,
            SkillGrant = Map( patron.SkillGrant ),
            Benefits = patron.Benefits.Select( Map ).ToArray(),
            SelectedFamiliarSpell = Map( selectedFamiliarSpell ),
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

    private static WitchPatronBenefitDto Map( WitchPatronBenefitDescriptor benefit )
    {
        return new WitchPatronBenefitDto
        {
            Id = benefit.Id,
            Kind = benefit.Kind,
            Name = benefit.Name,
            Summary = benefit.Summary,
            DeferredDependencies = benefit.DeferredDependencies.ToArray(),
        };
    }
}
