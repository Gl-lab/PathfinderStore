using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class SpellDefinitionDtoMapper
{
    public static SpellDefinitionDto Map( SpellDefinition spell )
    {
        ArgumentNullException.ThrowIfNull( spell );

        return new SpellDefinitionDto
        {
            Id = spell.Id,
            Name = spell.Name,
            Rank = spell.Rank,
            Kind = spell.Kind,
            Traditions = spell.Traditions,
            Traits = spell.Traits,
            Rarity = spell.Rarity,
            Source = new SourceReferenceDto
            {
                Book = spell.Source.Book,
                Page = spell.Source.Page,
            },
        };
    }
}
