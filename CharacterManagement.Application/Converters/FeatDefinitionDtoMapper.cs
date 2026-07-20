using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class FeatDefinitionDtoMapper
{
    public static FeatDefinitionDto Map( FeatDefinition feat )
    {
        ArgumentNullException.ThrowIfNull( feat );

        return new FeatDefinitionDto
        {
            Id = feat.Id,
            Name = feat.Name,
            Category = feat.Category,
            Level = feat.Level,
            Traits = feat.Traits,
            Rarity = feat.Rarity,
            Prerequisites = feat.Prerequisites,
            Summary = feat.Summary,
            DeferredDependencies = feat.DeferredDependencies,
            Source = new SourceReferenceDto
            {
                Book = feat.Source.Book,
                Page = feat.Source.Page,
            },
        };
    }
}
