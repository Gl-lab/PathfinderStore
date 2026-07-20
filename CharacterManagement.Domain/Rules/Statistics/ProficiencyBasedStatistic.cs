using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Statistics;

public sealed record ProficiencyBasedStatistic(
    AbilityType Ability,
    int AbilityModifier,
    ProficiencyRank ProficiencyRank,
    int ProficiencyBonus,
    int Total,
    IReadOnlyList<string> SourceGrantIds )
{
    public static ProficiencyBasedStatistic Calculate(
        AbilityType ability,
        Characteristic characteristic,
        EffectiveProficiency proficiency,
        int level )
    {
        ArgumentNullException.ThrowIfNull( characteristic );
        ArgumentNullException.ThrowIfNull( proficiency );

        int proficiencyBonus = ProficiencyBonusCalculator.Calculate( proficiency.Rank, level );

        return new ProficiencyBasedStatistic(
            ability,
            characteristic.Modifier,
            proficiency.Rank,
            proficiencyBonus,
            characteristic.Modifier + proficiencyBonus,
            proficiency.SourceGrantIds );
    }
}

public static class ProficiencyBonusCalculator
{
    public static int Calculate( ProficiencyRank rank, int level )
    {
        if ( level <= 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( level ), "Level must be greater than zero." );
        }

        int rankBonus = rank switch
                        {
                            ProficiencyRank.Untrained => 0,
                            ProficiencyRank.Trained => 2,
                            ProficiencyRank.Expert => 4,
                            ProficiencyRank.Master => 6,
                            ProficiencyRank.Legendary => 8,
                            _ => throw new ArgumentOutOfRangeException( nameof( rank ), rank, null )
                        };

        return rank == ProficiencyRank.Untrained ? 0 : level + rankBonus;
    }
}
