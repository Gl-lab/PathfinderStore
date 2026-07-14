namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum BardMuseBenefitKind
{
    ClassFeat,
    RepertoireSpell
}

public sealed class BardMuseBenefitDescriptor
{
    public string Id { get; }
    public BardMuseBenefitKind Kind { get; }
    public string Name { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public BardMuseBenefitDescriptor(
        string id,
        BardMuseBenefitKind kind,
        string name,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Bard Muse benefit id cannot be empty.", nameof( id ) );
        }

        if ( !Enum.IsDefined( kind ) )
        {
            throw new ArgumentOutOfRangeException( nameof( kind ), kind, "Unknown Bard Muse benefit kind." );
        }

        string requiredPrefix = kind == BardMuseBenefitKind.ClassFeat ? "feat." : "spell.";
        if ( !id.StartsWith( requiredPrefix, StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                $"Bard Muse {kind} benefit id must use the '{requiredPrefix}' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Bard Muse benefit name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( deferredDependencies );

        Id = id.Trim();
        Kind = kind;
        Name = name.Trim();
        DeferredDependencies = deferredDependencies.Distinct().ToArray();
    }
}

public sealed class BardMuse
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public IReadOnlyList<BardMuseBenefitDescriptor> Benefits { get; }

    public BardMuse(
        string id,
        string name,
        SourceReference source,
        IReadOnlyList<BardMuseBenefitDescriptor> benefits )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "bard_muse.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Bard Muse id must use the 'bard_muse.' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Bard Muse name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( benefits );

        if ( benefits.Count != 2 ||
             benefits.Select( benefit => benefit.Kind ).Distinct().Count() != 2 )
        {
            throw new ArgumentException(
                "Bard Muse must define one class feat and one repertoire spell.",
                nameof( benefits ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        Benefits = benefits.ToArray();
    }
}
