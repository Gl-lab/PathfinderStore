using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public enum CharacterCompletionIssueCode
{
    Identity,
    AncestryPackage,
    BackgroundPackage,
    ClassPackage,
    ClassChoices,
    SpellLoadout,
    FeatChoices,
    ClassTraining,
    Languages,
    FinalBoosts,
    StartingEquipment,
}

public sealed class CharacterCompletionIssueDto
{
    public CharacterCompletionIssueCode Code { get; set; }
    public string Message { get; set; } = String.Empty;
}

public sealed class CharacterCompletionDto
{
    public bool IsComplete { get; set; }
    public IReadOnlyList<CharacterCompletionIssueDto> Issues { get; set; } = [];
}

public sealed class CharacterCreationStateDto
{
    public CharacterCreationStatus CreationStatus { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public CharacterCompletionDto Completion { get; set; } = new CharacterCompletionDto();
}
