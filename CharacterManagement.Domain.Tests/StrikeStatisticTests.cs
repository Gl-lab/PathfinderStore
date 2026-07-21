using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Domain.Tests;

public sealed class StrikeStatisticTests
{
    [Fact]
    public void Calculate_FinesseMeleeStrike_UsesDexterityForAttackAndStrengthForDamage()
    {
        AbilityScores scores = AbilityScores.CreateDefault();
        scores.ApplyAbilityBoost( AbilityType.Dexterity );
        EffectiveProficiency proficiency = new EffectiveProficiency(
            ProficiencyTargets.SimpleWeapons,
            ProficiencyRank.Trained,
            [ "class.test" ] );
        StrikeProfile profile = new StrikeProfile(
            "strike.dagger.melee",
            "Dagger",
            StrikeKind.Weapon,
            StrikeMode.Melee,
            4,
            "Piercing",
            [ "Agile", "Finesse" ] );

        StrikeStatistic result = StrikeStatistic.Calculate( profile, scores, proficiency, [], [], 1 );

        Assert.Equal( AbilityType.Dexterity, result.Attack.Ability );
        Assert.Equal( 4, result.Attack.Total );
        Assert.Equal( AbilityType.Strength, result.Damage.Ability );
        Assert.Equal( "1d4 Piercing", result.Damage.Formula );
    }

    [Fact]
    public void Calculate_ThrownRangedStrike_UsesDexterityForAttackAndStrengthForDamage()
    {
        AbilityScores scores = AbilityScores.CreateDefault();
        scores.ApplyAbilityBoost( AbilityType.Strength );
        EffectiveProficiency proficiency = new EffectiveProficiency(
            ProficiencyTargets.SimpleWeapons,
            ProficiencyRank.Trained,
            [ "class.test" ] );
        StrikeProfile profile = new StrikeProfile(
            "strike.javelin.ranged",
            "Javelin",
            StrikeKind.Weapon,
            StrikeMode.Ranged,
            6,
            "Piercing",
            [ "Thrown" ] );

        StrikeStatistic result = StrikeStatistic.Calculate( profile, scores, proficiency, [], [], 1 );

        Assert.Equal( AbilityType.Dexterity, result.Attack.Ability );
        Assert.Equal( 3, result.Attack.Total );
        Assert.Equal( 1, result.Damage.AbilityModifier );
        Assert.Equal( "1d6+1 Piercing", result.Damage.Formula );
    }

    [Fact]
    public void Calculate_PropulsiveStrike_UsesHalfPositiveStrengthModifierForDamage()
    {
        AbilityScores scores = AbilityScores.CreateDefault();
        scores.ApplyAbilityBoost( AbilityType.Strength );
        scores.ApplyAbilityBoost( AbilityType.Strength );
        EffectiveProficiency proficiency = new EffectiveProficiency(
            ProficiencyTargets.SimpleWeapons,
            ProficiencyRank.Trained,
            [ "class.test" ] );
        StrikeProfile profile = new StrikeProfile(
            "strike.sling.ranged",
            "Sling",
            StrikeKind.Weapon,
            StrikeMode.Ranged,
            6,
            "Bludgeoning",
            [ "Propulsive" ] );

        StrikeStatistic result = StrikeStatistic.Calculate( profile, scores, proficiency, [], [], 1 );

        Assert.Equal( 1, result.Damage.AbilityModifier );
        Assert.Equal( "1d6+1 Bludgeoning", result.Damage.Formula );
    }
}
