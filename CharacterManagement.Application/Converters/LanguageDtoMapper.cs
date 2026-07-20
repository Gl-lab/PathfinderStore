using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class LanguageDtoMapper
{
    public static LanguageDto Map( LanguageDefinition language )
    {
        ArgumentNullException.ThrowIfNull( language );

        return new LanguageDto
        {
            Id = language.Id.Value,
            Name = language.Name,
            Speakers = language.Speakers,
            Rarity = language.Rarity,
            Category = language.Category,
            Source = language.Source,
        };
    }
}
