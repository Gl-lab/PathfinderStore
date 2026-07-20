using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterDerivedStatisticsDtoMapper
{
    private const int CharacterLevel = 1;

    public static CharacterDerivedStatisticsDto Map(
        DraftCharacter character,
        Ancestry ancestry,
        CharacterClass characterClass,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        ISkillRepository? skillRepository )
    {
        ArgumentNullException.ThrowIfNull( proficiencies );

        CharacterHitPoints hitPoints = CharacterHitPoints.Calculate(
            character,
            ancestry,
            characterClass );

        return new CharacterDerivedStatisticsDto
        {
            HitPoints = new CharacterHitPointsDto
            {
                Maximum = hitPoints.MaximumHitPoints,
                Ancestry = hitPoints.AncestryHitPoints,
                Class = hitPoints.ClassHitPoints,
                ConstitutionModifier = hitPoints.ConstitutionModifier,
            },
            Perception = Map(
                character,
                proficiencies,
                ProficiencyTargets.Perception,
                AbilityType.Wisdom ),
            SavingThrows = new CharacterSavingThrowsDto
            {
                Fortitude = Map(
                    character,
                    proficiencies,
                    ProficiencyTargets.Fortitude,
                    AbilityType.Constitution ),
                Reflex = Map(
                    character,
                    proficiencies,
                    ProficiencyTargets.Reflex,
                    AbilityType.Dexterity ),
                Will = Map(
                    character,
                    proficiencies,
                    ProficiencyTargets.Will,
                    AbilityType.Wisdom ),
            },
            SkillModifiers = CharacterSkillModifiersDtoMapper.Map(
                character,
                skillRepository ),
        };
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        ProficiencyTarget target,
        AbilityType ability )
    {
        EffectiveProficiency proficiency = proficiencies
            .Single( item => item.Target.Id == target.Id );
        ProficiencyBasedStatistic statistic = ProficiencyBasedStatistic.Calculate(
            ability,
            character.AbilityScores.GetCharacteristic( ability ),
            proficiency,
            CharacterLevel );

        return new CharacterProficiencyStatisticDto
        {
            TargetId = target.Id,
            Name = target.Name,
            Ability = statistic.Ability,
            AbilityModifier = statistic.AbilityModifier,
            ProficiencyRank = statistic.ProficiencyRank,
            ProficiencyBonus = statistic.ProficiencyBonus,
            Total = statistic.Total,
            SourceGrantIds = statistic.SourceGrantIds,
        };
    }
}
