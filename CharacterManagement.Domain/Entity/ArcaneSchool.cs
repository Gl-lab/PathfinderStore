namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum ArcaneSchoolBenefitKind
{
    InitialSchoolSpell,
    AdvancedSchoolSpell,
    ExtraClassFeat,
    ExtraSpellbookSpellChoice,
    AdditionalDrainBondedItemUses
}

public sealed class ArcaneSchoolCurriculumSpellDescriptor
{
    public string Id { get; }
    public string Name { get; }
    public int Rank { get; }
    public bool IsUncommon { get; }

    public ArcaneSchoolCurriculumSpellDescriptor(
        string id,
        string name,
        int rank,
        bool isUncommon = false )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "spell.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Curriculum spell id must use the 'spell.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Curriculum spell name cannot be empty.", nameof( name ) );
        }

        if ( rank < 0 || rank > 9 )
        {
            throw new ArgumentOutOfRangeException( nameof( rank ), rank, "Spell rank must be between 0 and 9." );
        }

        Id = id.Trim();
        Name = name.Trim();
        Rank = rank;
        IsUncommon = isUncommon;
    }
}

public sealed class ArcaneSchoolBenefitDescriptor
{
    public string Id { get; }
    public ArcaneSchoolBenefitKind Kind { get; }
    public string Name { get; }
    public string Summary { get; }
    public IReadOnlyList<CharacterClassDependencyType> DeferredDependencies { get; }

    public ArcaneSchoolBenefitDescriptor(
        string id,
        ArcaneSchoolBenefitKind kind,
        string name,
        string summary,
        IReadOnlyList<CharacterClassDependencyType> deferredDependencies )
    {
        if ( !Enum.IsDefined( kind ) )
        {
            throw new ArgumentOutOfRangeException( nameof( kind ), kind, "Unknown Arcane School benefit kind." );
        }

        string requiredPrefix = kind is ArcaneSchoolBenefitKind.InitialSchoolSpell or
            ArcaneSchoolBenefitKind.AdvancedSchoolSpell
            ? "spell."
            : "arcane_school.";
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( requiredPrefix, StringComparison.Ordinal ) )
        {
            throw new ArgumentException(
                $"Arcane School benefit id must use the '{requiredPrefix}' prefix.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) || String.IsNullOrWhiteSpace( summary ) )
        {
            throw new ArgumentException( "Arcane School benefit name and summary cannot be empty." );
        }

        ArgumentNullException.ThrowIfNull( deferredDependencies );

        Id = id.Trim();
        Kind = kind;
        Name = name.Trim();
        Summary = summary.Trim();
        DeferredDependencies = deferredDependencies.Distinct().ToArray();
    }
}

public sealed class ArcaneSchool
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public bool HasCurriculum => CurriculumSpells.Count > 0;
    public IReadOnlyList<ArcaneSchoolCurriculumSpellDescriptor> CurriculumSpells { get; }
    public IReadOnlyList<ArcaneSchoolBenefitDescriptor> Benefits { get; }

    public ArcaneSchool(
        string id,
        string name,
        SourceReference source,
        IReadOnlyList<ArcaneSchoolCurriculumSpellDescriptor> curriculumSpells,
        IReadOnlyList<ArcaneSchoolBenefitDescriptor> benefits )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "arcane_school.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Arcane School id must use the 'arcane_school.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Arcane School name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( curriculumSpells );
        ArgumentNullException.ThrowIfNull( benefits );

        ValidateCurriculum( curriculumSpells );
        ValidateBenefits( curriculumSpells.Count > 0, benefits );

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        CurriculumSpells = curriculumSpells.ToArray();
        Benefits = benefits.ToArray();
    }

    private static void ValidateCurriculum(
        IReadOnlyList<ArcaneSchoolCurriculumSpellDescriptor> curriculumSpells )
    {
        if ( curriculumSpells.Select( spell => spell.Id ).Distinct( StringComparer.Ordinal ).Count() !=
             curriculumSpells.Count )
        {
            throw new ArgumentException( "Curriculum spell ids must be unique.", nameof( curriculumSpells ) );
        }

        if ( curriculumSpells.Count > 0 &&
             !Enumerable.Range( 0, 10 ).All( rank => curriculumSpells.Any( spell => spell.Rank == rank ) ) )
        {
            throw new ArgumentException( "A curriculum must define spells for ranks 0 through 9.", nameof( curriculumSpells ) );
        }
    }

    private static void ValidateBenefits(
        bool hasCurriculum,
        IReadOnlyList<ArcaneSchoolBenefitDescriptor> benefits )
    {
        if ( benefits.Count( benefit => benefit.Kind == ArcaneSchoolBenefitKind.InitialSchoolSpell ) != 1 ||
             benefits.Count( benefit => benefit.Kind == ArcaneSchoolBenefitKind.AdvancedSchoolSpell ) != 1 )
        {
            throw new ArgumentException(
                "Arcane School must define one initial and one advanced school spell.",
                nameof( benefits ) );
        }

        ArcaneSchoolBenefitKind[] unifiedKinds =
        [
            ArcaneSchoolBenefitKind.ExtraClassFeat,
            ArcaneSchoolBenefitKind.ExtraSpellbookSpellChoice,
            ArcaneSchoolBenefitKind.AdditionalDrainBondedItemUses,
        ];
        bool hasAllUnifiedBenefits = unifiedKinds.All(
            kind => benefits.Count( benefit => benefit.Kind == kind ) == 1 );
        bool hasAnyUnifiedBenefit = unifiedKinds.Any(
            kind => benefits.Any( benefit => benefit.Kind == kind ) );
        if ( ( hasCurriculum && hasAnyUnifiedBenefit ) ||
             ( !hasCurriculum && !hasAllUnifiedBenefits ) )
        {
            throw new ArgumentException(
                "Only a no-curriculum school must define all Unified Magical Theory benefits.",
                nameof( benefits ) );
        }

        if ( benefits.Select( benefit => benefit.Id ).Distinct( StringComparer.Ordinal ).Count() != benefits.Count )
        {
            throw new ArgumentException( "Arcane School benefit ids must be unique.", nameof( benefits ) );
        }
    }
}
