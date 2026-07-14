using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class ClericDomainDtoMapper
{
    public static ClericDomainDto Map( ClericDomain clericDomain )
    {
        ArgumentNullException.ThrowIfNull( clericDomain );

        return new ClericDomainDto
        {
            Id = clericDomain.Id,
            Name = clericDomain.Name,
            Source = new SourceReferenceDto
            {
                Book = clericDomain.Source.Book,
                Page = clericDomain.Source.Page,
            },
            InitialFocusSpell = Map( clericDomain.InitialFocusSpell ),
        };
    }

    public static ClericDomainPackageDto MapPackage( ClericDomain clericDomain )
    {
        ArgumentNullException.ThrowIfNull( clericDomain );

        return new ClericDomainPackageDto
        {
            Id = clericDomain.Id,
            Name = clericDomain.Name,
            InitialFocusSpell = Map( clericDomain.InitialFocusSpell ),
        };
    }

    private static SpellReferenceDto Map( SpellReference spell )
    {
        return new SpellReferenceDto
        {
            Id = spell.Id,
            Name = spell.Name,
            Rank = spell.Rank,
            Kind = spell.Kind,
        };
    }
}
