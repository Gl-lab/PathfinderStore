namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum BackgroundGrantKind
{
    SkillTraining,
    LoreTraining,
    SkillFeat
}

public enum BackgroundDependencyType
{
    SkillCatalog,
    LoreCatalog,
    SkillFeatCatalog,
    ProficiencyRules,
    ClassCatalog
}

public sealed record BackgroundGrantOption(
    string Id,
    string Name );

public sealed record BackgroundGrantDescriptor(
    string Id,
    BackgroundGrantKind Kind,
    string Name,
    string Summary,
    bool RequiresChoice,
    bool AllowsCustomLore,
    string? TargetId,
    IReadOnlyList<BackgroundGrantOption> Options,
    IReadOnlyList<BackgroundDependencyType> DeferredDependencies );
