using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class WitchPatronRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsSevenPlayerCorePatrons()
    {
        WitchPatronRepository repository = new WitchPatronRepository();

        IReadOnlyCollection<WitchPatron> patrons = repository.GetAll();

        Assert.Equal( 7, patrons.Count );
        Assert.Equal(
            [
                "witch_patron.faiths_flamekeeper",
                "witch_patron.inscribed_one",
                "witch_patron.resentment",
                "witch_patron.silence_in_snow",
                "witch_patron.spinner_of_threads",
                "witch_patron.starless_shadow",
                "witch_patron.wilding_steward",
            ],
            patrons.Select( patron => patron.Id ).OrderBy( id => id ) );
    }

    [Fact]
    public void GetWitchPatron_FaithsFlamekeeper_ReturnsVerifiedPackage()
    {
        WitchPatronRepository repository = new WitchPatronRepository();

        WitchPatron patron = repository.GetWitchPatron( "witch_patron.faiths_flamekeeper" );

        Assert.Equal( SpellTradition.Divine, patron.SpellTradition );
        Assert.Equal( [ "skill.religion" ], patron.SkillGrant.SkillOptions );
        Assert.Contains( patron.Benefits, benefit => benefit.Id == "lesson.fervors_grasp" );
        Assert.Contains( patron.Benefits, benefit => benefit.Id == "spell.stoke_the_heart" );
        Assert.Contains( patron.Benefits, benefit => benefit.Id == "spell.command" );
        Assert.Contains( patron.Benefits, benefit => benefit.Id == "familiar_ability.restored_spirit" );
        Assert.Equal( 184, patron.Source.Page );
    }

    [Fact]
    public void GetWitchPatron_WildingSteward_ReturnsNestedSpellChoice()
    {
        WitchPatronRepository repository = new WitchPatronRepository();

        WitchPatron patron = repository.GetWitchPatron( "witch_patron.wilding_steward" );

        Assert.Equal( SpellTradition.Primal, patron.SpellTradition );
        Assert.Equal( [ "skill.nature" ], patron.SkillGrant.SkillOptions );
        Assert.Equal(
            [ "spell.summon_animal", "spell.summon_plant_or_fungus" ],
            patron.FamiliarSpellOptions.Select( spell => spell.Id ) );
        Assert.Equal( 185, patron.Source.Page );
    }
}
