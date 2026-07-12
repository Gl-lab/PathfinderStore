namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum VisionType
{
    None,
    LowLight,
    Darkvision
}

public enum AdditionalLanguageRuleType
{
    IntelligenceModifier,
    OnePlusIntelligenceModifier
}

public enum AncestryChoiceRarity
{
    Common,
    Uncommon
}

public enum AncestryEffectKind
{
    RuleEffect,
    VisionOverride,
    BaseHpOverride,
    DeferredChoice
}

public enum AncestryDependencyType
{
    RuleEngine,
    SpellCatalog,
    Spellcasting,
    ClassCatalog,
    ClassFeatCatalog,
    GeneralFeatCatalog,
    SkillCatalog,
    LoreCatalog,
    WeaponCatalog,
    ProficiencyRules,
    InventoryCatalog,
    LanguageCatalog,
    CombatRules,
    MovementRules,
    ConditionRules,
    ResistanceRules,
    EnvironmentRules,
    PerceptionRules,
    AnimalCompanionRules,
    ArchetypeCatalog
}

public readonly record struct LanguageId( string Value );

public sealed record SourceReference( string Book, int Page )
{
    public static SourceReference Unknown { get; } = new( "Unknown", 0 );
}

public sealed record AdditionalLanguageRule(
    AdditionalLanguageRuleType Type,
    IReadOnlyList<LanguageId> AllowedLanguageIds,
    bool UsesCommonAndUncommonLanguages );

public sealed record GrantedItem( string ItemId, int Quantity, SourceReference Source );

public sealed record GrantedRule( string RuleId, AncestryEffectKind EffectKind, string Summary );

public sealed record AncestryEffectDescriptor(
    string EffectId,
    AncestryEffectKind EffectKind,
    string Summary,
    VisionType? VisionOverride = null,
    int? BaseHitPointsOverride = null );

public sealed record Heritage(
    string Id,
    AncestryType AncestryType,
    string Name,
    SourceReference Source,
    AncestryChoiceRarity Rarity,
    IReadOnlyList<string> Restrictions,
    IReadOnlyList<string> IncompatibleChoiceIds,
    IReadOnlyList<AncestryEffectDescriptor> Effects,
    IReadOnlyList<AncestryDependencyType> DeferredDependencies );

public sealed record AncestryFeat(
    string Id,
    AncestryType AncestryType,
    string Name,
    SourceReference Source,
    int Level,
    AncestryChoiceRarity Rarity,
    IReadOnlyList<string> Prerequisites,
    IReadOnlyList<string> Restrictions,
    IReadOnlyList<string> IncompatibleChoiceIds,
    IReadOnlyList<AncestryEffectDescriptor> Effects,
    IReadOnlyList<AncestryDependencyType> DeferredDependencies );
