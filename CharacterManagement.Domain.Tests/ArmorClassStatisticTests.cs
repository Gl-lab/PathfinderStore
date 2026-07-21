using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Domain.Tests;

public sealed class ArmorClassStatisticTests
{
    [Fact]
    public void Calculate_ArmorCapsDexterityAndSeparatesBonusLayers()
    {
        Characteristic dexterity = new Characteristic( 18 );
        EffectiveProficiency proficiency = new EffectiveProficiency(
            ProficiencyTargets.LightArmor,
            ProficiencyRank.Trained,
            [ "class.rogue.initial_proficiencies" ] );

        ArmorClassStatistic result = ArmorClassStatistic.Calculate(
            dexterity,
            proficiency,
            3,
            [
                new StatisticBonus( "equipment.studded_leather_armor", StatisticBonusType.Item, 2 ),
                new StatisticBonus( "effect.test_status", StatisticBonusType.Status, 1 ),
                new StatisticBonus( "effect.test_circumstance", StatisticBonusType.Circumstance, 1 ),
            ],
            1 );

        Assert.Equal( 20, result.Total );
        Assert.Equal( 4, result.AbilityModifier );
        Assert.Equal( 3, result.AppliedAbilityModifier );
        Assert.Equal( 3, result.ProficiencyBonus );
        Assert.Single( result.ItemBonuses );
        Assert.Single( result.StatusBonuses );
        Assert.Single( result.CircumstanceBonuses );
    }

    [Fact]
    public void Calculate_UnarmoredWithoutBonuses_UsesFullDexterityModifier()
    {
        Characteristic dexterity = new Characteristic( 14 );
        EffectiveProficiency proficiency = new EffectiveProficiency(
            ProficiencyTargets.UnarmoredDefense,
            ProficiencyRank.Trained,
            [ "class.wizard.initial_proficiencies" ] );

        ArmorClassStatistic result = ArmorClassStatistic.Calculate(
            dexterity,
            proficiency,
            null,
            [],
            1 );

        Assert.Equal( 15, result.Total );
        Assert.Null( result.AbilityCap );
        Assert.Equal( result.AbilityModifier, result.AppliedAbilityModifier );
        Assert.Empty( result.ItemBonuses );
        Assert.Empty( result.StatusBonuses );
        Assert.Empty( result.CircumstanceBonuses );
    }
}
