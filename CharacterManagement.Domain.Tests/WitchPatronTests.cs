using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class WitchPatronTests
{
    [Fact]
    public void Constructor_ValidPatron_PreservesTypedBenefits()
    {
        WitchPatron patron = CreatePatron( [ ( "command", "Command" ) ] );

        Assert.Contains( patron.Benefits, benefit => benefit.Kind == WitchPatronBenefitKind.Lesson );
        Assert.Contains( patron.Benefits, benefit => benefit.Kind == WitchPatronBenefitKind.HexCantrip );
        Assert.Contains( patron.Benefits, benefit => benefit.Kind == WitchPatronBenefitKind.FamiliarAbility );
        Assert.Single( patron.FamiliarSpellOptions );
    }

    [Fact]
    public void ResolveFamiliarSpell_SingleOption_DerivesSpellAndRejectsPayload()
    {
        WitchPatron patron = CreatePatron( [ ( "command", "Command" ) ] );

        WitchPatronBenefitDescriptor selectedSpell = patron.ResolveFamiliarSpell( null );

        Assert.Equal( "spell.command", selectedSpell.Id );
        Assert.Throws<CharacterManagementException>( () => patron.ResolveFamiliarSpell( "spell.command" ) );
    }

    [Fact]
    public void ResolveFamiliarSpell_MultipleOptions_RequiresAllowedChoice()
    {
        WitchPatron patron = CreatePatron(
            [
                ( "summon_animal", "Summon Animal" ),
                ( "summon_plant_or_fungus", "Summon Plant or Fungus" ),
            ] );

        WitchPatronBenefitDescriptor selectedSpell = patron.ResolveFamiliarSpell( "spell.summon_animal" );

        Assert.Equal( "spell.summon_animal", selectedSpell.Id );
        Assert.Throws<CharacterManagementException>( () => patron.ResolveFamiliarSpell( null ) );
        Assert.Throws<CharacterManagementException>( () => patron.ResolveFamiliarSpell( "spell.command" ) );
    }

    [Fact]
    public void Constructor_MissingRequiredBenefitKind_Throws()
    {
        Assert.Throws<ArgumentException>( () => new WitchPatron(
            "witch_patron.faiths_flamekeeper",
            "Faith's Flamekeeper",
            SourceReference.Unknown,
            SpellTradition.Divine,
            new ClassSkillGrantDescriptor(
                "witch_patron.faiths_flamekeeper.skill.patron",
                [ "skill.religion" ] ),
            [
                Benefit( "lesson.fervors_grasp", WitchPatronBenefitKind.Lesson, "Fervor's Grasp" ),
                Benefit( "spell.command", WitchPatronBenefitKind.FamiliarSpell, "Command" ),
                Benefit( "familiar_ability.restored_spirit", WitchPatronBenefitKind.FamiliarAbility, "Restored Spirit" ),
            ] ) );
    }

    private static WitchPatron CreatePatron( IReadOnlyList<(string Id, string Name)> familiarSpells )
    {
        return new WitchPatron(
            "witch_patron.faiths_flamekeeper",
            "Faith's Flamekeeper",
            SourceReference.Unknown,
            SpellTradition.Divine,
            new ClassSkillGrantDescriptor(
                "witch_patron.faiths_flamekeeper.skill.patron",
                [ "skill.religion" ] ),
            [
                Benefit( "lesson.fervors_grasp", WitchPatronBenefitKind.Lesson, "Fervor's Grasp" ),
                Benefit( "spell.stoke_the_heart", WitchPatronBenefitKind.HexCantrip, "Stoke the Heart" ),
                .. familiarSpells.Select( spell => Benefit(
                    $"spell.{spell.Id}",
                    WitchPatronBenefitKind.FamiliarSpell,
                    spell.Name ) ),
                Benefit( "familiar_ability.restored_spirit", WitchPatronBenefitKind.FamiliarAbility, "Restored Spirit" ),
            ] );
    }

    private static WitchPatronBenefitDescriptor Benefit(
        string id,
        WitchPatronBenefitKind kind,
        string name )
    {
        return new WitchPatronBenefitDescriptor(
            id,
            kind,
            name,
            name,
            [ CharacterClassDependencyType.ClassFeatureRules ] );
    }
}
