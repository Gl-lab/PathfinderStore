using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class ArcaneThesisRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsFivePlayerCoreTheses()
    {
        ArcaneThesisRepository repository = new ArcaneThesisRepository();

        IReadOnlyCollection<ArcaneThesis> theses = repository.GetAll();

        Assert.Equal( 5, theses.Count );
        Assert.Equal(
            [
                "arcane_thesis.experimental_spellshaping",
                "arcane_thesis.improved_familiar_attunement",
                "arcane_thesis.spell_blending",
                "arcane_thesis.spell_substitution",
                "arcane_thesis.staff_nexus",
            ],
            theses.Select( thesis => thesis.Id ).OrderBy( id => id ) );
        Assert.All( theses, thesis => Assert.Equal( 195, thesis.Source.Page ) );
    }

    [Fact]
    public void GetArcaneThesis_ImprovedFamiliar_ReturnsVerifiedProgression()
    {
        ArcaneThesisRepository repository = new ArcaneThesisRepository();

        ArcaneThesis thesis = repository.GetArcaneThesis(
            "arcane_thesis.improved_familiar_attunement" );

        ArcaneThesisEffectDescriptor progression = Assert.Single(
            thesis.Effects.Where(
                effect => effect.Kind == ArcaneThesisEffectKind.FamiliarAbilityProgression ) );
        Assert.Equal( [ 1, 6, 12, 18 ], progression.MilestoneLevels );
        Assert.Contains( thesis.Effects, effect =>
            effect.Kind == ArcaneThesisEffectKind.DrainFamiliarReplacement );
    }

    [Fact]
    public void GetArcaneThesis_StaffNexus_ReturnsTypedDeferredEffects()
    {
        ArcaneThesisRepository repository = new ArcaneThesisRepository();

        ArcaneThesis thesis = repository.GetArcaneThesis( "arcane_thesis.staff_nexus" );

        Assert.Contains( thesis.Effects, effect =>
            effect.Kind == ArcaneThesisEffectKind.MakeshiftStaff );
        ArcaneThesisEffectDescriptor progression = Assert.Single(
            thesis.Effects.Where(
                effect => effect.Kind == ArcaneThesisEffectKind.StaffChargeProgression ) );
        Assert.Equal( [ 8, 16 ], progression.MilestoneLevels );
        Assert.Contains(
            CharacterClassDependencyType.ItemCatalog,
            progression.DeferredDependencies );
    }
}
