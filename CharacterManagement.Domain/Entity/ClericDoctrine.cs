namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed class ClericDoctrine
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public IReadOnlyList<ProficiencyGrant> ProficiencyGrants { get; }
    public IReadOnlyList<ClericDoctrineEffectDescriptor> Effects { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public ClericDoctrine(
        string id,
        string name,
        SourceReference source,
        IReadOnlyList<ProficiencyGrant> proficiencyGrants,
        IReadOnlyList<ClericDoctrineEffectDescriptor> effects,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "cleric_doctrine.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Cleric doctrine id must use the 'cleric_doctrine.' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Cleric doctrine name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( proficiencyGrants );
        ArgumentNullException.ThrowIfNull( effects );
        ArgumentNullException.ThrowIfNull( deferredDependencies );

        if ( proficiencyGrants
                 .Select( grant => grant.SourceGrantId )
                 .Distinct( StringComparer.Ordinal )
                 .Count() != proficiencyGrants.Count )
        {
            throw new ArgumentException(
                "Cleric doctrine proficiency source ids must be unique.",
                nameof( proficiencyGrants ) );
        }

        if ( effects
                 .Select( effect => effect.Id )
                 .Distinct( StringComparer.Ordinal )
                 .Count() != effects.Count )
        {
            throw new ArgumentException( "Cleric doctrine effect ids must be unique.", nameof( effects ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        ProficiencyGrants = proficiencyGrants.ToArray();
        Effects = effects.ToArray();
        DeferredDependencies = deferredDependencies
            .Distinct()
            .ToArray();
    }
}

public sealed class ClericDoctrineEffectDescriptor
{
    public string Id { get; }
    public string Name { get; }
    public string Summary { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public ClericDoctrineEffectDescriptor(
        string id,
        string name,
        string summary,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "cleric_doctrine.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Cleric doctrine effect id must use the 'cleric_doctrine.' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Cleric doctrine effect name cannot be empty.", nameof( name ) );
        }

        if ( String.IsNullOrWhiteSpace( summary ) )
        {
            throw new ArgumentException( "Cleric doctrine effect summary cannot be empty.", nameof( summary ) );
        }

        ArgumentNullException.ThrowIfNull( deferredDependencies );

        Id = id.Trim();
        Name = name.Trim();
        Summary = summary.Trim();
        DeferredDependencies = deferredDependencies
            .Distinct()
            .ToArray();
    }
}
