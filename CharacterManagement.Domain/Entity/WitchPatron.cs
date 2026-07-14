using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum WitchPatronBenefitKind
{
    Lesson,
    HexCantrip,
    FamiliarSpell,
    FamiliarAbility
}

public sealed class WitchPatronBenefitDescriptor
{
    public string Id { get; }
    public WitchPatronBenefitKind Kind { get; }
    public string Name { get; }
    public string Summary { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public WitchPatronBenefitDescriptor(
        string id,
        WitchPatronBenefitKind kind,
        string name,
        string summary,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Witch Patron benefit id cannot be empty.", nameof( id ) );
        }

        if ( !Enum.IsDefined( kind ) )
        {
            throw new ArgumentOutOfRangeException( nameof( kind ), kind, "Unknown Witch Patron benefit kind." );
        }

        string requiredPrefix = kind switch
        {
            WitchPatronBenefitKind.Lesson => "lesson.",
            WitchPatronBenefitKind.HexCantrip => "spell.",
            WitchPatronBenefitKind.FamiliarSpell => "spell.",
            WitchPatronBenefitKind.FamiliarAbility => "familiar_ability.",
            _ => throw new ArgumentOutOfRangeException( nameof( kind ), kind, null ),
        };
        if ( !id.StartsWith( requiredPrefix, StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                $"Witch Patron {kind} benefit id must use the '{requiredPrefix}' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) || String.IsNullOrWhiteSpace( summary ) )
        {
            throw new ArgumentException( "Witch Patron benefit name and summary cannot be empty." );
        }

        ArgumentNullException.ThrowIfNull( deferredDependencies );

        Id = id.Trim();
        Kind = kind;
        Name = name.Trim();
        Summary = summary.Trim();
        DeferredDependencies = deferredDependencies.Distinct().ToArray();
    }
}

public sealed class WitchPatron
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public SpellTradition SpellTradition { get; }
    public ClassSkillGrantDescriptor SkillGrant { get; }
    public IReadOnlyList<WitchPatronBenefitDescriptor> Benefits { get; }
    public IReadOnlyList<WitchPatronBenefitDescriptor> FamiliarSpellOptions { get; }

    public WitchPatron(
        string id,
        string name,
        SourceReference source,
        SpellTradition spellTradition,
        ClassSkillGrantDescriptor skillGrant,
        IReadOnlyList<WitchPatronBenefitDescriptor> benefits )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "witch_patron.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                "Witch Patron id must use the 'witch_patron.' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Witch Patron name cannot be empty.", nameof( name ) );
        }

        if ( !Enum.IsDefined( spellTradition ) )
        {
            throw new ArgumentOutOfRangeException( nameof( spellTradition ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( skillGrant );
        ArgumentNullException.ThrowIfNull( benefits );

        string normalizedId = id.Trim();
        if ( !skillGrant.Id.StartsWith( $"{normalizedId}.skill.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Witch Patron skill grant must belong to the Patron.", nameof( skillGrant ) );
        }

        ValidateBenefits( benefits );

        Id = normalizedId;
        Name = name.Trim();
        Source = source;
        SpellTradition = spellTradition;
        SkillGrant = skillGrant;
        Benefits = benefits.ToArray();
        FamiliarSpellOptions = Benefits
            .Where( benefit => benefit.Kind == WitchPatronBenefitKind.FamiliarSpell )
            .ToArray();
    }

    public WitchPatronBenefitDescriptor ResolveFamiliarSpell( string? familiarSpellId )
    {
        if ( FamiliarSpellOptions.Count == 1 )
        {
            if ( !String.IsNullOrWhiteSpace( familiarSpellId ) )
            {
                throw new CharacterManagementException(
                    "A familiar spell choice is not allowed when the Patron grants a single spell." );
            }

            return FamiliarSpellOptions[ 0 ];
        }

        if ( String.IsNullOrWhiteSpace( familiarSpellId ) )
        {
            throw new CharacterManagementException( "Witch Patron requires a familiar spell choice." );
        }

        WitchPatronBenefitDescriptor? selectedSpell = FamiliarSpellOptions
            .SingleOrDefault( benefit => benefit.Id == familiarSpellId );
        return selectedSpell ?? throw new CharacterManagementException(
            $"Familiar spell '{familiarSpellId}' is not allowed for Patron '{Id}'." );
    }

    private static void ValidateBenefits( IReadOnlyList<WitchPatronBenefitDescriptor> benefits )
    {
        WitchPatronBenefitKind[] singleKinds =
        [
            WitchPatronBenefitKind.Lesson,
            WitchPatronBenefitKind.HexCantrip,
            WitchPatronBenefitKind.FamiliarAbility,
        ];
        if ( singleKinds.Any( kind => benefits.Count( benefit => benefit.Kind == kind ) != 1 ) ||
             benefits.Count( benefit => benefit.Kind == WitchPatronBenefitKind.FamiliarSpell ) == 0 )
        {
            throw new ArgumentException(
                "Witch Patron must define one lesson, one hex cantrip, familiar spells, and one familiar ability.",
                nameof( benefits ) );
        }

        if ( benefits.Select( benefit => benefit.Id ).Distinct( StringComparer.Ordinal ).Count() != benefits.Count )
        {
            throw new ArgumentException( "Witch Patron benefit ids must be unique.", nameof( benefits ) );
        }
    }
}
