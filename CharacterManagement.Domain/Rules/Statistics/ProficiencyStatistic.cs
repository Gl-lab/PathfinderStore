using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Statistics;

public enum ProficiencyStatisticKind
{
    Modifier,
    DifficultyClass
}

public sealed record ProficiencyStatistic(
    ProficiencyStatisticKind Kind,
    int Base,
    AbilityType Ability,
    int AbilityModifier,
    string ProficiencyTargetId,
    ProficiencyRank ProficiencyRank,
    int ProficiencyBonus,
    IReadOnlyList<string> ProficiencySourceGrantIds,
    IReadOnlyList<StatisticBonus> ItemBonuses,
    IReadOnlyList<StatisticBonus> StatusBonuses,
    IReadOnlyList<StatisticBonus> CircumstanceBonuses,
    int Total )
{
    public static ProficiencyStatistic Calculate(
        ProficiencyStatisticKind kind,
        AbilityType ability,
        Characteristic characteristic,
        EffectiveProficiency proficiency,
        IReadOnlyList<StatisticBonus> bonuses,
        int level )
    {
        ArgumentNullException.ThrowIfNull( characteristic );
        ArgumentNullException.ThrowIfNull( proficiency );
        ArgumentNullException.ThrowIfNull( bonuses );

        if ( !Enum.IsDefined( kind ) )
        {
            throw new ArgumentOutOfRangeException( nameof( kind ) );
        }

        if ( bonuses.Any( bonus => String.IsNullOrWhiteSpace( bonus.SourceId ) ) )
        {
            throw new ArgumentException( "Statistic bonus source ids cannot be empty.", nameof( bonuses ) );
        }

        int baseValue = kind == ProficiencyStatisticKind.DifficultyClass ? 10 : 0;
        int proficiencyBonus = ProficiencyBonusCalculator.Calculate( proficiency.Rank, level );
        return new ProficiencyStatistic(
            kind,
            baseValue,
            ability,
            characteristic.Modifier,
            proficiency.Target.Id,
            proficiency.Rank,
            proficiencyBonus,
            proficiency.SourceGrantIds.ToArray(),
            bonuses.Where( bonus => bonus.Type == StatisticBonusType.Item ).ToArray(),
            bonuses.Where( bonus => bonus.Type == StatisticBonusType.Status ).ToArray(),
            bonuses.Where( bonus => bonus.Type == StatisticBonusType.Circumstance ).ToArray(),
            baseValue + characteristic.Modifier + proficiencyBonus + bonuses.Sum( bonus => bonus.Value ) );
    }
}
