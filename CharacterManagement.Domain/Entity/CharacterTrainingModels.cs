namespace Pathfinder.CharacterManagement.Domain.Entity;

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
    IReadOnlyList<TrainedLore> Lore );
