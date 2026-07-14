namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum DruidicOrderBenefitKind
{
    ClassFeat,
    FocusSpell
}

public sealed class DruidicOrderBenefitDescriptor
{
    public string Id { get; }
    public DruidicOrderBenefitKind Kind { get; }
    public string Name { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public DruidicOrderBenefitDescriptor(
        string id,
        DruidicOrderBenefitKind kind,
        string name,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Druidic Order benefit id cannot be empty.", nameof( id ) );
        }

        if ( !Enum.IsDefined( kind ) )
        {
            throw new ArgumentOutOfRangeException( nameof( kind ), kind, "Unknown Druidic Order benefit kind." );
        }

        string requiredPrefix = kind == DruidicOrderBenefitKind.ClassFeat ? "feat." : "spell.";
        if ( !id.StartsWith( requiredPrefix, StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                $"Druidic Order {kind} benefit id must use the '{requiredPrefix}' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Druidic Order benefit name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( deferredDependencies );

        Id = id.Trim();
        Kind = kind;
        Name = name.Trim();
        DeferredDependencies = deferredDependencies.Distinct().ToArray();
    }
}

public sealed class DruidicOrder
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public ClassSkillGrantDescriptor SkillGrant { get; }
    public IReadOnlyList<DruidicOrderBenefitDescriptor> Benefits { get; }

    public DruidicOrder(
        string id,
        string name,
        SourceReference source,
        ClassSkillGrantDescriptor skillGrant,
        IReadOnlyList<DruidicOrderBenefitDescriptor> benefits )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "druidic_order.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Druidic Order id must use the 'druidic_order.' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Druidic Order name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( skillGrant );
        ArgumentNullException.ThrowIfNull( benefits );

        string normalizedId = id.Trim();
        if ( !skillGrant.Id.StartsWith( $"{normalizedId}.skill.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Druidic Order skill grant must belong to the Order.",
                nameof( skillGrant ) );
        }

        if ( benefits.Count != 2 ||
             benefits.Select( benefit => benefit.Kind ).Distinct().Count() != 2 )
        {
            throw new ArgumentException(
                "Druidic Order must define one class feat and one focus spell.",
                nameof( benefits ) );
        }

        Id = normalizedId;
        Name = name.Trim();
        Source = source;
        SkillGrant = skillGrant;
        Benefits = benefits.ToArray();
    }
}
