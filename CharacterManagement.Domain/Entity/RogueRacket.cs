namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed class RogueRacket
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public AbilityType? AlternativeKeyAbility { get; }
    public IReadOnlyList<RogueSkillGrantDescriptor> SkillGrants { get; }
    public IReadOnlyList<ProficiencyGrant> ProficiencyGrants { get; }
    public IReadOnlyList<RogueRacketEffectDescriptor> Effects { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public RogueRacket(
        string id,
        string name,
        SourceReference source,
        AbilityType? alternativeKeyAbility,
        IReadOnlyList<RogueSkillGrantDescriptor> skillGrants,
        IReadOnlyList<ProficiencyGrant> proficiencyGrants,
        IReadOnlyList<RogueRacketEffectDescriptor> effects,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "rogue_racket.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Rogue racket id must use the 'rogue_racket.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Rogue racket name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( skillGrants );
        ArgumentNullException.ThrowIfNull( proficiencyGrants );
        ArgumentNullException.ThrowIfNull( effects );
        ArgumentNullException.ThrowIfNull( deferredDependencies );

        if ( alternativeKeyAbility.HasValue && !Enum.IsDefined( alternativeKeyAbility.Value ) )
        {
            throw new ArgumentOutOfRangeException( nameof( alternativeKeyAbility ) );
        }

        if ( skillGrants.Select( grant => grant.Id ).Distinct( StringComparer.Ordinal ).Count() != skillGrants.Count )
        {
            throw new ArgumentException( "Rogue racket skill grant ids must be unique.", nameof( skillGrants ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        AlternativeKeyAbility = alternativeKeyAbility;
        SkillGrants = skillGrants.ToArray();
        ProficiencyGrants = proficiencyGrants.ToArray();
        Effects = effects.ToArray();
        DeferredDependencies = deferredDependencies.ToArray();
    }
}

public sealed class RogueSkillGrantDescriptor
{
    public string Id { get; }
    public string? TargetId { get; }
    public IReadOnlyList<string> Options { get; }
    public bool RequiresChoice => Options.Count > 0;

    public RogueSkillGrantDescriptor(
        string id,
        string? targetId,
        IReadOnlyList<string> options )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             ( !id.StartsWith( "rogue_racket.", StringComparison.Ordinal ) &&
               !id.StartsWith( "class.rogue.", StringComparison.Ordinal ) ) )
        {
            throw new ArgumentException(
                "Rogue skill grant id must use the 'rogue_racket.' or 'class.rogue.' prefix.",
                nameof( id ) );
        }

        ArgumentNullException.ThrowIfNull( options );

        bool hasTarget = !String.IsNullOrWhiteSpace( targetId );
        bool hasOptions = options.Count > 0;
        if ( hasTarget == hasOptions )
        {
            throw new ArgumentException( "Rogue skill grant must define either one target or a finite option list." );
        }

        if ( options.Any( option => String.IsNullOrWhiteSpace( option ) ) ||
             options.Distinct( StringComparer.Ordinal ).Count() != options.Count )
        {
            throw new ArgumentException( "Rogue skill grant options must be non-empty and unique.", nameof( options ) );
        }

        Id = id.Trim();
        TargetId = targetId?.Trim();
        Options = options.ToArray();
    }
}

public sealed record RogueRacketEffectDescriptor(
    string Id,
    string Name,
    string Summary );
