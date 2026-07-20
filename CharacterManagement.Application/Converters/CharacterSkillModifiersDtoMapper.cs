using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;
using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterSkillModifiersDtoMapper
{
    public static CharacterSkillModifiersDto Map(
        DraftCharacter character,
        ISkillRepository? skillRepository,
        int characterLevel,
        IReadOnlyList<TrainedSkill>? effectiveSkills = null,
        IReadOnlyList<TrainedLore>? effectiveLore = null )
    {
        ArgumentNullException.ThrowIfNull( character );

        if ( skillRepository is null )
        {
            throw new InvalidOperationException( "Skill repository is required to map skill modifiers." );
        }

        IReadOnlyDictionary<string, TrainedSkill> trainedSkills = ( effectiveSkills ?? character.TrainedSkills )
            .ToDictionary( training => training.SkillId, StringComparer.Ordinal );

        return new CharacterSkillModifiersDto
        {
            General = skillRepository
                .GetAll()
                .OrderBy( skill => skill.Name, StringComparer.Ordinal )
                .Select( skill => Map(
                    character,
                    skill,
                    trainedSkills,
                    characterLevel ) )
                .ToArray(),
            Lore = ( effectiveLore ?? character.TrainedLore )
                .OrderBy( lore => lore.Name, StringComparer.Ordinal )
                .Select( lore => Map( character, lore, characterLevel ) )
                .ToArray(),
        };
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        SkillDefinition skill,
        IReadOnlyDictionary<string, TrainedSkill> trainedSkills,
        int characterLevel )
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
            sourceGrantIds,
            characterLevel );
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        TrainedLore lore,
        int characterLevel )
    {
        return Map(
            character,
            lore.LoreId,
            lore.Name,
            AbilityType.Intelligence,
            ProficiencyRank.Trained,
            [ lore.SourceGrantId ],
            characterLevel );
    }

    private static CharacterProficiencyStatisticDto Map(
        DraftCharacter character,
        string id,
        string name,
        AbilityType ability,
        ProficiencyRank rank,
        IReadOnlyList<string> sourceGrantIds,
        int characterLevel )
    {
        ProficiencyBasedStatistic statistic = ProficiencyBasedStatistic.Calculate(
            ability,
            character.AbilityScores.GetCharacteristic( ability ),
            rank,
            sourceGrantIds,
            characterLevel );

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
