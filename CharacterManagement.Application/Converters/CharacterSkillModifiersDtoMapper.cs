using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;
using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterSkillModifiersDtoMapper
{
    private const int CharacterLevel = 1;

    public static CharacterSkillModifiersDto Map(
        DraftCharacter character,
        ISkillRepository? skillRepository )
    {
        ArgumentNullException.ThrowIfNull( character );

        if ( skillRepository is null )
        {
            throw new InvalidOperationException( "Skill repository is required to map skill modifiers." );
        }

        IReadOnlyDictionary<string, TrainedSkill> trainedSkills = character.TrainedSkills
            .ToDictionary( training => training.SkillId, StringComparer.Ordinal );

        return new CharacterSkillModifiersDto
        {
            General = skillRepository
                .GetAll()
                .OrderBy( skill => skill.Name, StringComparer.Ordinal )
                .Select( skill => Map( character, skill, trainedSkills ) )
                .ToArray(),
            Lore = character.TrainedLore
                .OrderBy( lore => lore.Name, StringComparer.Ordinal )
                .Select( lore => Map( character, lore ) )
                .ToArray(),
        };
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        SkillDefinition skill,
        IReadOnlyDictionary<string, TrainedSkill> trainedSkills )
    {
        bool isTrained = trainedSkills.TryGetValue( skill.Id, out TrainedSkill? training );
        ProficiencyRank rank = isTrained ? ProficiencyRank.Trained : ProficiencyRank.Untrained;
        IReadOnlyList<string> sourceGrantIds = training is null ? [] : [ training.SourceGrantId ];

        return Map(
            character,
            skill.Id,
            skill.Name,
            skill.KeyAbility,
            rank,
            sourceGrantIds );
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        TrainedLore lore )
    {
        return Map(
            character,
            lore.LoreId,
            lore.Name,
            AbilityType.Intelligence,
            ProficiencyRank.Trained,
            [ lore.SourceGrantId ] );
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        string id,
        string name,
        AbilityType ability,
        ProficiencyRank rank,
        IReadOnlyList<string> sourceGrantIds )
    {
        ProficiencyBasedStatistic statistic = ProficiencyBasedStatistic.Calculate(
            ability,
            character.AbilityScores.GetCharacteristic( ability ),
            rank,
            sourceGrantIds,
            CharacterLevel );

        return new CharacterProficiencyStatisticDto
        {
            TargetId = id,
            Name = name,
            Ability = statistic.Ability,
            AbilityModifier = statistic.AbilityModifier,
            ProficiencyRank = statistic.ProficiencyRank,
            ProficiencyBonus = statistic.ProficiencyBonus,
            Total = statistic.Total,
            SourceGrantIds = statistic.SourceGrantIds,
        };
    }
}
