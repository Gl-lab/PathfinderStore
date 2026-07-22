using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Statistics;

public enum StrikeKind
{
    Weapon,
    Unarmed
}

public enum StrikeMode
{
    Melee,
    Ranged
}

public sealed record StrikeProfile(
    string Id,
    string Name,
    StrikeKind Kind,
    StrikeMode Mode,
    int DamageDie,
    string DamageType,
    IReadOnlyList<string> Traits );

public sealed record StrikeAttackStatistic(
    AbilityType Ability,
    int AbilityModifier,
    string ProficiencyTargetId,
    ProficiencyRank ProficiencyRank,
    int ProficiencyBonus,
    IReadOnlyList<string> ProficiencySourceGrantIds,
    IReadOnlyList<StatisticBonus> ItemBonuses,
    IReadOnlyList<StatisticBonus> StatusBonuses,
    IReadOnlyList<StatisticBonus> CircumstanceBonuses,
    int Total );

public sealed record StrikeDamageStatistic(
    int DiceCount,
    int Die,
    string DamageType,
    AbilityType? Ability,
    int AbilityModifier,
    IReadOnlyList<StatisticBonus> ItemBonuses,
    IReadOnlyList<StatisticBonus> StatusBonuses,
    IReadOnlyList<StatisticBonus> CircumstanceBonuses,
    string Formula );

public sealed record StrikeStatistic(
    StrikeProfile Profile,
    StrikeAttackStatistic Attack,
    StrikeDamageStatistic Damage )
{
    public static StrikeStatistic Calculate(
        StrikeProfile profile,
        AbilityScores abilityScores,
        EffectiveProficiency proficiency,
        IReadOnlyList<StatisticBonus> attackBonuses,
        IReadOnlyList<StatisticBonus> damageBonuses,
        int level )
    {
        ArgumentNullException.ThrowIfNull( profile );
        ArgumentNullException.ThrowIfNull( abilityScores );
        ArgumentNullException.ThrowIfNull( proficiency );
        ArgumentNullException.ThrowIfNull( attackBonuses );
        ArgumentNullException.ThrowIfNull( damageBonuses );

        if ( attackBonuses.Concat( damageBonuses ).Any( bonus => String.IsNullOrWhiteSpace( bonus.SourceId ) ) )
        {
            throw new ArgumentException( "Statistic bonus source ids cannot be empty." );
        }

        AbilityType attackAbility = ResolveAttackAbility( profile, abilityScores );
        int attackAbilityModifier = abilityScores
            .GetCharacteristic( attackAbility )
            .Modifier;
        int proficiencyBonus = ProficiencyBonusCalculator.Calculate( proficiency.Rank, level );
        int damageAbilityModifier = ResolveDamageAbilityModifier( profile, abilityScores.Strength.Modifier );
        AbilityType? damageAbility = damageAbilityModifier == 0 &&
                                     profile.Mode == StrikeMode.Ranged &&
                                     !HasTrait( profile, "Thrown" ) &&
                                     !HasTrait( profile, "Propulsive" )
            ? null
            : AbilityType.Strength;
        StrikeAttackStatistic attack = new StrikeAttackStatistic(
            attackAbility,
            attackAbilityModifier,
            proficiency.Target.Id,
            proficiency.Rank,
            proficiencyBonus,
            proficiency.SourceGrantIds.ToArray(),
            FilterBonuses( attackBonuses, StatisticBonusType.Item ),
            FilterBonuses( attackBonuses, StatisticBonusType.Status ),
            FilterBonuses( attackBonuses, StatisticBonusType.Circumstance ),
            attackAbilityModifier + proficiencyBonus + attackBonuses.Sum( bonus => bonus.Value ) );
        StrikeDamageStatistic damage = new StrikeDamageStatistic(
            1,
            profile.DamageDie,
            profile.DamageType,
            damageAbility,
            damageAbilityModifier,
            FilterBonuses( damageBonuses, StatisticBonusType.Item ),
            FilterBonuses( damageBonuses, StatisticBonusType.Status ),
            FilterBonuses( damageBonuses, StatisticBonusType.Circumstance ),
            FormatDamageFormula(
                profile.DamageDie,
                damageAbilityModifier + damageBonuses.Sum( bonus => bonus.Value ),
                profile.DamageType ) );

        return new StrikeStatistic( profile, attack, damage );
    }

    private static AbilityType ResolveAttackAbility( StrikeProfile profile, AbilityScores abilityScores )
    {
        if ( profile.Mode == StrikeMode.Ranged )
        {
            return AbilityType.Dexterity;
        }

        return HasTrait( profile, "Finesse" ) &&
               abilityScores.Dexterity.Modifier > abilityScores.Strength.Modifier
            ? AbilityType.Dexterity
            : AbilityType.Strength;
    }

    private static int ResolveDamageAbilityModifier( StrikeProfile profile, int strengthModifier )
    {
        if ( profile.Mode == StrikeMode.Melee || HasTrait( profile, "Thrown" ) )
        {
            return strengthModifier;
        }

        if ( HasTrait( profile, "Propulsive" ) )
        {
            return strengthModifier > 0 ? strengthModifier / 2 : strengthModifier;
        }

        return 0;
    }

    private static bool HasTrait( StrikeProfile profile, string trait )
    {
        return profile.Traits.Any( item => item.StartsWith( trait, StringComparison.OrdinalIgnoreCase ) );
    }

    private static IReadOnlyList<StatisticBonus> FilterBonuses(
        IReadOnlyList<StatisticBonus> bonuses,
        StatisticBonusType type )
    {
        return bonuses.Where( bonus => bonus.Type == type ).ToArray();
    }

    private static string FormatDamageFormula( int die, int modifier, string damageType )
    {
        string modifierText = modifier switch
        {
            > 0 => $"+{modifier}",
            < 0 => modifier.ToString(),
            _ => String.Empty,
        };
        return $"1d{die}{modifierText} {damageType}";
    }
}
