using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Statistics;

public sealed record ArmorClassStatistic(
    int Base,
    AbilityType Ability,
    int AbilityModifier,
    int? AbilityCap,
    int AppliedAbilityModifier,
    string ProficiencyTargetId,
    ProficiencyRank ProficiencyRank,
    int ProficiencyBonus,
    IReadOnlyList<string> ProficiencySourceGrantIds,
    IReadOnlyList<StatisticBonus> ItemBonuses,
    IReadOnlyList<StatisticBonus> StatusBonuses,
    IReadOnlyList<StatisticBonus> CircumstanceBonuses,
    int Total )
{
    public const int BaseArmorClass = 10;

    public static ArmorClassStatistic Calculate(
        Characteristic dexterity,
        EffectiveProficiency proficiency,
        int? dexterityCap,
        IReadOnlyList<StatisticBonus> bonuses,
        int level )
    {
        ArgumentNullException.ThrowIfNull( dexterity );
        ArgumentNullException.ThrowIfNull( proficiency );
        ArgumentNullException.ThrowIfNull( bonuses );

        if ( dexterityCap < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( dexterityCap ) );
        }

        if ( bonuses.Any( bonus => String.IsNullOrWhiteSpace( bonus.SourceId ) ) )
        {
            throw new ArgumentException( "Statistic bonus source ids cannot be empty.", nameof( bonuses ) );
        }

        int appliedDexterityModifier = dexterityCap.HasValue
            ? Math.Min( dexterity.Modifier, dexterityCap.Value )
            : dexterity.Modifier;
        int proficiencyBonus = ProficiencyBonusCalculator.Calculate( proficiency.Rank, level );
        IReadOnlyList<StatisticBonus> itemBonuses = bonuses
            .Where( bonus => bonus.Type == StatisticBonusType.Item )
            .ToArray();
        IReadOnlyList<StatisticBonus> statusBonuses = bonuses
            .Where( bonus => bonus.Type == StatisticBonusType.Status )
            .ToArray();
        IReadOnlyList<StatisticBonus> circumstanceBonuses = bonuses
            .Where( bonus => bonus.Type == StatisticBonusType.Circumstance )
            .ToArray();
        int total = BaseArmorClass + appliedDexterityModifier + proficiencyBonus + bonuses.Sum( bonus => bonus.Value );

        return new ArmorClassStatistic(
            BaseArmorClass,
            AbilityType.Dexterity,
            dexterity.Modifier,
            dexterityCap,
            appliedDexterityModifier,
            proficiency.Target.Id,
            proficiency.Rank,
            proficiencyBonus,
            proficiency.SourceGrantIds.ToArray(),
            itemBonuses,
            statusBonuses,
            circumstanceBonuses,
            total );
    }
}
