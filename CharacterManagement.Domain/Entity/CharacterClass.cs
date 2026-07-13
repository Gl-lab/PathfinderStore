namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed class CharacterClass
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public int BaseHitPoints { get; }
    public IReadOnlyList<AbilityType> KeyAbilityOptions { get; }
    public IReadOnlyList<ProficiencyGrant> InitialProficiencies { get; }
    public SpellTradition? SpellTradition { get; }
    public IReadOnlyList<CharacterClassRuleDescriptor> Rules { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public CharacterClass(
        string id,
        string name,
        SourceReference source,
        int baseHitPoints,
        IReadOnlyList<AbilityType> keyAbilityOptions,
        IReadOnlyList<ProficiencyGrant> initialProficiencies,
        SpellTradition? spellTradition,
        IReadOnlyList<CharacterClassRuleDescriptor> rules,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Character class id cannot be empty.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Character class name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( keyAbilityOptions );
        ArgumentNullException.ThrowIfNull( initialProficiencies );
        ArgumentNullException.ThrowIfNull( rules );
        ArgumentNullException.ThrowIfNull( deferredDependencies );

        if ( baseHitPoints <= 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( baseHitPoints ), "Base hit points must be greater than zero." );
        }

        if ( keyAbilityOptions.Count == 0 )
        {
            throw new ArgumentException( "Character class must define a key ability option.", nameof( keyAbilityOptions ) );
        }

        if ( keyAbilityOptions.Distinct().Count() != keyAbilityOptions.Count )
        {
            throw new ArgumentException( "Character class key ability options must be unique.", nameof( keyAbilityOptions ) );
        }

        if ( initialProficiencies.Count == 0 )
        {
            throw new ArgumentException(
                "Character class must define initial proficiencies.",
                nameof( initialProficiencies ) );
        }

        if ( initialProficiencies
            .Select( grant => grant.Target.Id )
            .Distinct( StringComparer.Ordinal )
            .Count() != initialProficiencies.Count )
        {
            throw new ArgumentException(
                "Character class initial proficiency targets must be unique.",
                nameof( initialProficiencies ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        BaseHitPoints = baseHitPoints;
        KeyAbilityOptions = keyAbilityOptions;
        InitialProficiencies = initialProficiencies.ToArray();
        SpellTradition = spellTradition;
        Rules = rules;
        DeferredDependencies = deferredDependencies;
    }
}
