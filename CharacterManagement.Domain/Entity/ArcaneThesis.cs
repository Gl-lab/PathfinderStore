namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum ArcaneThesisEffectKind
{
    FirstLevelSpellshapeFeatChoice,
    DailySpellshapeFeatChoice,
    FamiliarFeatGrant,
    FamiliarAbilityProgression,
    DrainFamiliarReplacement,
    SpellSlotBlending,
    PreparedSpellSubstitution,
    MakeshiftStaff,
    StaffChargePreparation,
    StaffChargeProgression
}

public sealed class ArcaneThesisEffectDescriptor
{
    public string Id { get; }
    public ArcaneThesisEffectKind Kind { get; }
    public string Name { get; }
    public string Summary { get; }
    public IReadOnlyList<int> MilestoneLevels { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public ArcaneThesisEffectDescriptor(
        string id,
        ArcaneThesisEffectKind kind,
        string name,
        string summary,
        IReadOnlyList<int> milestoneLevels,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( !Enum.IsDefined( kind ) )
        {
            throw new ArgumentOutOfRangeException( nameof( kind ), kind, "Unknown Arcane Thesis effect kind." );
        }

        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "arcane_thesis.", StringComparison.Ordinal ) ||
             !id.Contains( ".effect.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Arcane Thesis effect id must use the 'arcane_thesis.<thesis>.effect.<effect>' shape.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) || String.IsNullOrWhiteSpace( summary ) )
        {
            throw new ArgumentException( "Arcane Thesis effect name and summary cannot be empty." );
        }

        ArgumentNullException.ThrowIfNull( milestoneLevels );
        ArgumentNullException.ThrowIfNull( deferredDependencies );

        int[] normalizedMilestones = milestoneLevels
            .Distinct()
            .OrderBy( level => level )
            .ToArray();
        if ( normalizedMilestones.Length == 0 ||
             normalizedMilestones.Any( level => level < 1 ) ||
             !normalizedMilestones.SequenceEqual( milestoneLevels ) )
        {
            throw new ArgumentException(
                "Arcane Thesis effect milestone levels must be positive, unique, and ascending.",
                nameof( milestoneLevels ) );
        }

        bool isResolvedFeatEffect = kind == ArcaneThesisEffectKind.FirstLevelSpellshapeFeatChoice ||
                                    kind == ArcaneThesisEffectKind.FamiliarFeatGrant;
        if ( ( !isResolvedFeatEffect && deferredDependencies.Count == 0 ) ||
             deferredDependencies.Any( dependency => !Enum.IsDefined( dependency ) ) )
        {
            throw new ArgumentException(
                "Unresolved Arcane Thesis effects must define known deferred dependencies.",
                nameof( deferredDependencies ) );
        }

        Id = id.Trim();
        Kind = kind;
        Name = name.Trim();
        Summary = summary.Trim();
        MilestoneLevels = normalizedMilestones;
        DeferredDependencies = deferredDependencies.Distinct().ToArray();
    }
}

public sealed class ArcaneThesis
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public IReadOnlyList<ArcaneThesisEffectDescriptor> Effects { get; }

    public ArcaneThesis(
        string id,
        string name,
        SourceReference source,
        IReadOnlyList<ArcaneThesisEffectDescriptor> effects )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "arcane_thesis.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Arcane Thesis id must use the 'arcane_thesis.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Arcane Thesis name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( effects );

        string normalizedId = id.Trim();
        if ( effects.Count == 0 )
        {
            throw new ArgumentException( "Arcane Thesis must define an effect.", nameof( effects ) );
        }

        if ( effects.Any( effect => !effect.Id.StartsWith(
                $"{normalizedId}.effect.",
                StringComparison.Ordinal ) ) )
        {
            throw new ArgumentException( "Arcane Thesis effects must belong to the Thesis.", nameof( effects ) );
        }

        if ( effects.Select( effect => effect.Id ).Distinct( StringComparer.Ordinal ).Count() != effects.Count ||
             effects.Select( effect => effect.Kind ).Distinct().Count() != effects.Count )
        {
            throw new ArgumentException( "Arcane Thesis effect ids and kinds must be unique.", nameof( effects ) );
        }

        Id = normalizedId;
        Name = name.Trim();
        Source = source;
        Effects = effects.ToArray();
    }
}
