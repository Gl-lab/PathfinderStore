using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Domain.Tests;

public sealed class ProficiencyStatisticTests
{
    [Theory]
    [InlineData( ProficiencyStatisticKind.Modifier, 7 )]
    [InlineData( ProficiencyStatisticKind.DifficultyClass, 17 )]
    public void Calculate_ModifierOrDifficultyClass_AppliesBaseAbilityAndProficiency(
        ProficiencyStatisticKind kind,
        int expectedTotal )
    {
        EffectiveProficiency proficiency = new EffectiveProficiency(
            ProficiencyTargets.ClassDc( "class.wizard", "Wizard" ),
            ProficiencyRank.Trained,
            [ "class.wizard.initial_proficiencies" ] );

        ProficiencyStatistic result = ProficiencyStatistic.Calculate(
            kind,
            AbilityType.Intelligence,
            new Characteristic( 18 ),
            proficiency,
            [],
            1 );

        Assert.Equal( expectedTotal, result.Total );
        Assert.Equal( kind == ProficiencyStatisticKind.DifficultyClass ? 10 : 0, result.Base );
        Assert.Equal( 4, result.AbilityModifier );
        Assert.Equal( 3, result.ProficiencyBonus );
        Assert.Empty( result.ItemBonuses );
        Assert.Empty( result.StatusBonuses );
        Assert.Empty( result.CircumstanceBonuses );
    }
}
