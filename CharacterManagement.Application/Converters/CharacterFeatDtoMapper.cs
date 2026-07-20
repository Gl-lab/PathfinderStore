using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterFeatDtoMapper
{
    public static CharacterFeatDto Map( CharacterFeat feat )
    {
        ArgumentNullException.ThrowIfNull( feat );

        return new CharacterFeatDto
        {
            Id = feat.Definition.Id,
            Name = feat.Definition.Name,
            Category = feat.Definition.Category,
            Level = feat.Definition.Level,
            Traits = feat.Definition.Traits,
            Prerequisites = feat.Definition.Prerequisites,
            Summary = feat.Definition.Summary,
            DeferredDependencies = feat.Definition.DeferredDependencies,
            Source = new SourceReferenceDto
            {
                Book = feat.Definition.Source.Book,
                Page = feat.Definition.Source.Page,
            },
            AcquisitionType = feat.AcquisitionType,
            SourceType = feat.SourceType,
            SourceId = feat.SourceId,
        };
    }
}
