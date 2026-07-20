namespace Pathfinder.CharacterManagement.Domain.Rules.Training;

public sealed record BackgroundTrainingChoice(
    string GrantId,
    string? TargetId,
    string? CustomLoreTopic );

public sealed record TrainedSkill(
    string SkillId,
    string SourceGrantId );

public sealed record TrainedLore(
    string LoreId,
    string Name,
    string SourceGrantId );

public sealed record BackgroundTrainingResult(
    IReadOnlyList<TrainedSkill> Skills,
    IReadOnlyList<TrainedLore> Lore,
    string? SkillFeatId );

public sealed record RogueTrainingChoice(
    string GrantId,
    string? SelectedSkillId,
    string? ReplacementSkillId );

public sealed record RogueTrainingResult(
    IReadOnlyList<TrainedSkill> Skills );

public sealed record ClassTrainingTargetChoice(
    string? SkillId,
    string? CustomLoreTopic );

public sealed record ClassSkillGrantChoice(
    string GrantId,
    string? SelectedSkillId,
    ClassTrainingTargetChoice? ReplacementTarget );

public sealed record ClassTrainingResult(
    IReadOnlyList<TrainedSkill> Skills,
    IReadOnlyList<TrainedLore> Lore );
