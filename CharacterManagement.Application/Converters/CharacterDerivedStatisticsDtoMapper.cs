using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterDerivedStatisticsDtoMapper
{
    private const int CharacterLevel = 1;

    public static CharacterDerivedStatisticsDto Map(
        DraftCharacter character,
        Ancestry ancestry,
        CharacterClass characterClass,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        ISkillRepository? skillRepository,
        FeatTrainingResult? featTraining = null,
        AllowedEquipmentLoadout? equipment = null )
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
            ArmorClass = MapArmorClass( character, proficiencies, equipment ),
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
                skillRepository,
                CharacterLevel,
                featTraining?.Skills,
                featTraining?.Lore ),
        };
    }

    private static CharacterArmorClassDto MapArmorClass(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        AllowedEquipmentLoadout? equipment )
    {
        AllowedEquipmentItem? equippedArmor = equipment?.Items
            .SingleOrDefault( item => ( item.EquippedQuantity > 0 ) && ( item.Armor is not null ) );
        string proficiencyTargetId = equippedArmor?.ProficiencyTargetId ?? ProficiencyTargets.UnarmoredDefense.Id;
        EffectiveProficiency proficiency = proficiencies
            .Single( item => item.Target.Id == proficiencyTargetId );
        IReadOnlyList<StatisticBonus> bonuses = equippedArmor?.Armor is null
            ? []
            :
            [
                new StatisticBonus(
                    equippedArmor.Id,
                    StatisticBonusType.Item,
                    equippedArmor.Armor.ArmorClassBonus ),
            ];
        ArmorClassStatistic armorClass = ArmorClassStatistic.Calculate(
            character.AbilityScores.Dexterity,
            proficiency,
            equippedArmor?.Armor?.DexterityCap,
            bonuses,
            CharacterLevel );

        return new CharacterArmorClassDto
        {
            Base = armorClass.Base,
            Ability = armorClass.Ability,
            AbilityModifier = armorClass.AbilityModifier,
            AbilityCap = armorClass.AbilityCap,
            AppliedAbilityModifier = armorClass.AppliedAbilityModifier,
            ProficiencyTargetId = armorClass.ProficiencyTargetId,
            ProficiencyRank = armorClass.ProficiencyRank,
            ProficiencyBonus = armorClass.ProficiencyBonus,
            ProficiencySourceGrantIds = armorClass.ProficiencySourceGrantIds,
            ItemBonuses = armorClass.ItemBonuses.Select( MapBonus ).ToArray(),
            StatusBonuses = armorClass.StatusBonuses.Select( MapBonus ).ToArray(),
            CircumstanceBonuses = armorClass.CircumstanceBonuses.Select( MapBonus ).ToArray(),
            Total = armorClass.Total,
        };
    }

    private static CharacterStatisticBonusDto MapBonus( StatisticBonus bonus )
    {
        return new CharacterStatisticBonusDto
        {
            SourceId = bonus.SourceId,
            Type = bonus.Type,
            Value = bonus.Value,
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
