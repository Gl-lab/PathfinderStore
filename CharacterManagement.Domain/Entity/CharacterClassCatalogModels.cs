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
    RogueRacketCatalog,
    ClericDoctrineCatalog,
    DomainCatalog,
    GeneralFeatCatalog,
    WeaponCatalog,
    SpellPreparationRules,
    SpellSlotRules,
    ItemCatalog
}

public sealed record CharacterClassRuleDescriptor(
    string Id,
    CharacterClassRuleKind Kind,
    string Name,
    string Summary,
    bool RequiresChoice,
    IReadOnlyList<CharacterClassDependencyType> DeferredDependencies );

public sealed class ClassSkillGrantDescriptor
{
    public string Id { get; }
    public IReadOnlyList<string> SkillOptions { get; }

    public ClassSkillGrantDescriptor( string id, IReadOnlyList<string> skillOptions )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Class skill grant id cannot be empty.", nameof( id ) );
        }

        ArgumentNullException.ThrowIfNull( skillOptions );

        if ( skillOptions.Count == 0 )
        {
            throw new ArgumentException( "Class skill grant must define at least one option.", nameof( skillOptions ) );
        }

        if ( skillOptions.Any( String.IsNullOrWhiteSpace ) )
        {
            throw new ArgumentException( "Class skill grant options cannot be empty.", nameof( skillOptions ) );
        }

        string[] normalizedSkillOptions = skillOptions
            .Select( option => option.Trim() )
            .ToArray();
        if ( normalizedSkillOptions.Any( option =>
                !option.StartsWith( "skill.", StringComparison.Ordinal ) ) )
        {
            throw new ArgumentException(
                "Class skill grant options must use the 'skill.' prefix.",
                nameof( skillOptions ) );
        }

        if ( normalizedSkillOptions
            .Distinct( StringComparer.Ordinal )
            .Count() != normalizedSkillOptions.Length )
        {
            throw new ArgumentException( "Class skill grant options must be unique.", nameof( skillOptions ) );
        }

        Id = id.Trim();
        SkillOptions = normalizedSkillOptions;
    }
}
