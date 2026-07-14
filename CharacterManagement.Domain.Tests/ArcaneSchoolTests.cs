using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class ArcaneSchoolTests
{
    [Fact]
    public void Constructor_NormalSchool_PreservesCurriculumAndSchoolSpells()
    {
        ArcaneSchool school = new ArcaneSchool(
            "arcane_school.test",
            "Test School",
            SourceReference.Unknown,
            CreateCurriculum(),
            [
                Benefit( "spell.initial", ArcaneSchoolBenefitKind.InitialSchoolSpell ),
                Benefit( "spell.advanced", ArcaneSchoolBenefitKind.AdvancedSchoolSpell ),
            ] );

        Assert.True( school.HasCurriculum );
        Assert.Equal( 10, school.CurriculumSpells.Count );
        Assert.Contains( school.Benefits, benefit => benefit.Kind == ArcaneSchoolBenefitKind.InitialSchoolSpell );
    }

    [Fact]
    public void Constructor_CurriculumMissingRank_Throws()
    {
        Assert.Throws<ArgumentException>( () => new ArcaneSchool(
            "arcane_school.test",
            "Test School",
            SourceReference.Unknown,
            CreateCurriculum().Where( spell => spell.Rank != 9 ).ToArray(),
            [
                Benefit( "spell.initial", ArcaneSchoolBenefitKind.InitialSchoolSpell ),
                Benefit( "spell.advanced", ArcaneSchoolBenefitKind.AdvancedSchoolSpell ),
            ] ) );
    }

    [Fact]
    public void Constructor_NoCurriculumRequiresUnifiedBenefits()
    {
        Assert.Throws<ArgumentException>( () => new ArcaneSchool(
            "arcane_school.test",
            "Test School",
            SourceReference.Unknown,
            [],
            [
                Benefit( "spell.initial", ArcaneSchoolBenefitKind.InitialSchoolSpell ),
                Benefit( "spell.advanced", ArcaneSchoolBenefitKind.AdvancedSchoolSpell ),
            ] ) );
    }

    [Fact]
    public void BenefitConstructor_UnknownKind_ThrowsArgumentOutOfRangeException()
    {
        ArcaneSchoolBenefitKind unknownKind = ( ArcaneSchoolBenefitKind )Int32.MaxValue;

        Assert.Throws<ArgumentOutOfRangeException>( () => new ArcaneSchoolBenefitDescriptor(
            "arcane_school.test.benefit.unknown",
            unknownKind,
            "Unknown",
            "Unknown benefit.",
            [] ) );
    }

    private static IReadOnlyList<ArcaneSchoolCurriculumSpellDescriptor> CreateCurriculum()
    {
        return Enumerable.Range( 0, 10 )
            .Select( rank => new ArcaneSchoolCurriculumSpellDescriptor(
                $"spell.rank_{rank}",
                $"Rank {rank}",
                rank ) )
            .ToArray();
    }

    private static ArcaneSchoolBenefitDescriptor Benefit(
        string id,
        ArcaneSchoolBenefitKind kind )
    {
        return new ArcaneSchoolBenefitDescriptor(
            id,
            kind,
            id,
            id,
            [ CharacterClassDependencyType.SpellCatalog ] );
    }
}
