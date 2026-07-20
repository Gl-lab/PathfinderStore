namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum FeatCategory
{
    Ancestry,
    Skill,
    Class
}

public enum FeatRarity
{
    Common,
    Uncommon
}

public enum FeatDependencyType
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
    FamiliarRules,
    FocusSpellRules,
    ArchetypeCatalog,
    FeatParameterChoice
}

public sealed class FeatDefinition
{
    public string Id { get; }
    public string Name { get; }
    public FeatCategory Category { get; }
    public int Level { get; }
    public IReadOnlyList<string> Traits { get; }
    public FeatRarity Rarity { get; }
    public IReadOnlyList<string> Prerequisites { get; }
    public string Summary { get; }
    public IReadOnlyList<FeatDependencyType> DeferredDependencies { get; }
    public SourceReference Source { get; }

    public FeatDefinition(
        string id,
        string name,
        FeatCategory category,
        int level,
        IReadOnlyList<string> traits,
        FeatRarity rarity,
        IReadOnlyList<string> prerequisites,
        string summary,
        IReadOnlyList<FeatDependencyType> deferredDependencies,
        SourceReference source )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Feat id cannot be empty.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Feat name cannot be empty.", nameof( name ) );
        }

        if ( level < 1 )
        {
            throw new ArgumentOutOfRangeException( nameof( level ), "Feat level must be positive." );
        }

        ArgumentNullException.ThrowIfNull( traits );
        ArgumentNullException.ThrowIfNull( prerequisites );
        ArgumentNullException.ThrowIfNull( deferredDependencies );
        ArgumentNullException.ThrowIfNull( source );

        if ( traits.Count == 0 )
        {
            throw new ArgumentException( "Feat must define at least one trait.", nameof( traits ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Category = category;
        Level = level;
        Traits = NormalizeStrings( traits );
        Rarity = rarity;
        Prerequisites = NormalizeStrings( prerequisites );
        Summary = summary.Trim();
        DeferredDependencies = deferredDependencies
            .Distinct()
            .ToArray();
        Source = source;
    }

    private static IReadOnlyList<string> NormalizeStrings( IReadOnlyList<string> values )
    {
        return values
            .Where( value => !String.IsNullOrWhiteSpace( value ) )
            .Select( value => value.Trim() )
            .Distinct( StringComparer.OrdinalIgnoreCase )
            .ToArray();
    }
}
