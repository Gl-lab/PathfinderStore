namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class LanguageSelectionOptionsDto
{
    public int RequiredCount { get; set; }
    public IReadOnlyList<LanguageDto> AvailableLanguages { get; set; } = [];
}
