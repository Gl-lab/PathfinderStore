using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace CharacterManagement.Domain.Tests;

public sealed class ProficiencyBasedStatisticTests
{
    [Theory]
    [InlineData( ProficiencyRank.Untrained, 0 )]
    [InlineData( ProficiencyRank.Trained, 3 )]
    [InlineData( ProficiencyRank.Expert, 5 )]
    [InlineData( ProficiencyRank.Master, 7 )]
    [InlineData( ProficiencyRank.Legendary, 9 )]
    public void Calculate_LevelOne_ReturnsExpectedProficiencyBonus(
        ProficiencyRank rank,
        int expectedBonus )
    {
        int result = ProficiencyBonusCalculator.Calculate( rank, 1 );

        Assert.Equal( expectedBonus, result );
    }

    [Theory]
    [InlineData( 0 )]
    [InlineData( -1 )]
    public void Calculate_NonPositiveLevel_Throws( int level )
    {
        Assert.Throws<ArgumentOutOfRangeException>( () =>
            ProficiencyBonusCalculator.Calculate( ProficiencyRank.Trained, level ) );
    }

    [Fact]
    public void Calculate_CombinesAbilityAndProficiencyBreakdown()
    {
        EffectiveProficiency proficiency = new EffectiveProficiency(
            ProficiencyTargets.Perception,
            ProficiencyRank.Expert,
            [ "class.fighter.initial_proficiencies" ] );

        ProficiencyBasedStatistic result = ProficiencyBasedStatistic.Calculate(
            AbilityType.Wisdom,
            new Characteristic( 14 ),
            proficiency,
            1 );

        Assert.Equal( AbilityType.Wisdom, result.Ability );
        Assert.Equal( 2, result.AbilityModifier );
        Assert.Equal( ProficiencyRank.Expert, result.ProficiencyRank );
        Assert.Equal( 5, result.ProficiencyBonus );
        Assert.Equal( 7, result.Total );
        Assert.Equal(
            [ "class.fighter.initial_proficiencies" ],
            result.SourceGrantIds );
    }

    [Theory]
    [InlineData( ProficiencyRank.Untrained, 1 )]
    [InlineData( ProficiencyRank.Trained, 4 )]
    public void Calculate_ExplicitRank_SupportsSkillModifiers(
        ProficiencyRank rank,
        int expectedTotal )
    {
        ProficiencyBasedStatistic result = ProficiencyBasedStatistic.Calculate(
            AbilityType.Dexterity,
            new Characteristic( 12 ),
            rank,
            rank == ProficiencyRank.Trained ? [ "training.source" ] : [],
            1 );

        Assert.Equal( expectedTotal, result.Total );
        Assert.Equal(
            rank == ProficiencyRank.Trained ? [ "training.source" ] : [],
            result.SourceGrantIds );
    }
}
