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
