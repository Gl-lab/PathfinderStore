namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum HuntersEdgeEffectKind
{
    MultipleAttackPenalty,
    ConditionalBonuses,
    PrecisionDamage
}

public sealed class HuntersEdgeEffectDescriptor
{
    public string Id { get; }
    public HuntersEdgeEffectKind Kind { get; }
    public string Name { get; }
    public string Summary { get; }

    public HuntersEdgeEffectDescriptor(
        string id,
        HuntersEdgeEffectKind kind,
        string name,
        string summary )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Hunter's Edge effect id cannot be empty.", nameof( id ) );
        }

        if ( !Enum.IsDefined( kind ) )
        {
            throw new ArgumentOutOfRangeException( nameof( kind ), kind, "Unknown Hunter's Edge effect kind." );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Hunter's Edge effect name cannot be empty.", nameof( name ) );
        }

        if ( String.IsNullOrWhiteSpace( summary ) )
        {
            throw new ArgumentException( "Hunter's Edge effect summary cannot be empty.", nameof( summary ) );
        }

        Id = id.Trim();
        Kind = kind;
        Name = name.Trim();
        Summary = summary.Trim();
    }
}

public sealed class HuntersEdge
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public IReadOnlyList<HuntersEdgeEffectDescriptor> Effects { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public HuntersEdge(
        string id,
        string name,
        SourceReference source,
        IReadOnlyList<HuntersEdgeEffectDescriptor> effects,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "hunters_edge.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Hunter's Edge id must use the 'hunters_edge.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Hunter's Edge name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( effects );
        ArgumentNullException.ThrowIfNull( deferredDependencies );

        if ( effects.Count == 0 )
        {
            throw new ArgumentException( "Hunter's Edge must define an effect.", nameof( effects ) );
        }

        if ( effects
            .Select( effect => effect.Id )
            .Distinct( StringComparer.Ordinal )
            .Count() != effects.Count )
        {
            throw new ArgumentException( "Hunter's Edge effect ids must be unique.", nameof( effects ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        Effects = effects.ToArray();
        DeferredDependencies = deferredDependencies.ToArray();
    }
}
