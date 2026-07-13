namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum SpellTradition
{
    Arcane,
    Divine,
    Occult,
    Primal
}

public enum CharacterClassRuleKind
{
    AdditionalSkills,
    GrantedFeature,
    MandatoryChoice,
    Spellcasting,
    ClassFeatChoice,
    SkillFeatChoice
}

public enum CharacterClassDependencyType
{
    ProficiencyRules,
    SkillCatalog,
    ClassFeatureRules,
    ClassChoiceCatalog,
    ClassFeatCatalog,
    SkillFeatCatalog,
    SpellCatalog,
    DeityCatalog,
    FamiliarRules,
    RogueRacketCatalog
}

public sealed record CharacterClassRuleDescriptor(
    string Id,
    CharacterClassRuleKind Kind,
    string Name,
    string Summary,
    bool RequiresChoice,
    IReadOnlyList<CharacterClassDependencyType> DeferredDependencies );
