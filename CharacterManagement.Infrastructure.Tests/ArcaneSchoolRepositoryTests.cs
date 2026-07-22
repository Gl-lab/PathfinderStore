using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class ArcaneSchoolRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsSevenPlayerCoreSchools()
    {
        ArcaneSchoolRepository repository = new ArcaneSchoolRepository();

        IReadOnlyCollection<ArcaneSchool> schools = repository.GetAll();

        Assert.Equal( 7, schools.Count );
        Assert.Equal(
            [
                "arcane_school.ars_grammatica",
                "arcane_school.battle_magic",
                "arcane_school.boundary",
                "arcane_school.civic_wizardry",
                "arcane_school.mentalism",
                "arcane_school.protean_form",
                "arcane_school.unified_magical_theory",
            ],
            schools.Select( school => school.Id ).OrderBy( id => id ) );
    }

    [Fact]
    public void GetArcaneSchool_ArsGrammatica_ReturnsVerifiedCurriculum()
    {
        ArcaneSchoolRepository repository = new ArcaneSchoolRepository();

        ArcaneSchool school = repository.GetArcaneSchool( "arcane_school.ars_grammatica" );

        Assert.Equal( 198, school.Source.Page );
        Assert.Contains( school.CurriculumSpells, spell =>
            spell.Id == "spell.veil_of_privacy" && spell.IsUncommon && spell.Rank == 3 );
        Assert.Contains( school.CurriculumSpells, spell =>
            spell.Id == "spell.detonate_magic" && spell.IsUncommon && spell.Rank == 9 );
        Assert.Contains( school.Benefits, benefit => benefit.Id == "spell.protective_wards" );
        Assert.Contains( school.Benefits, benefit => benefit.Id == "spell.rune_of_observation" );
    }

    [Fact]
    public void GetArcaneSchool_UnifiedMagicalTheory_ReturnsNoCurriculumAndDeferredBenefits()
    {
        ArcaneSchoolRepository repository = new ArcaneSchoolRepository();

        ArcaneSchool school = repository.GetArcaneSchool(
            "arcane_school.unified_magical_theory" );

        Assert.False( school.HasCurriculum );
        Assert.Empty( school.CurriculumSpells );
        Assert.Contains( school.Benefits, benefit => benefit.Kind == ArcaneSchoolBenefitKind.ExtraClassFeat );
        Assert.Contains( school.Benefits, benefit => benefit.Kind == ArcaneSchoolBenefitKind.ExtraSpellbookSpellChoice );
        Assert.Contains( school.Benefits, benefit => benefit.Kind == ArcaneSchoolBenefitKind.AdditionalDrainBondedItemUses );
    }
}
