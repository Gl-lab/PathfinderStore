using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterTrainingDtoMapper
{
    public static CharacterTrainingDto Map(
        DraftCharacter character,
        ISkillRepository? skillRepository,
        FeatTrainingResult? featTraining = null )
    {
        ArgumentNullException.ThrowIfNull( character );

        IReadOnlyList<TrainedSkill> trainedSkills = featTraining?.Skills ?? character.TrainedSkills;
        IReadOnlyList<TrainedLore> trainedLore = featTraining?.Lore ?? character.TrainedLore;
        if ( trainedSkills.Count > 0 && skillRepository is null )
        {
            throw new InvalidOperationException( "Skill repository is required to map trained skills." );
        }

        return new CharacterTrainingDto
        {
            Skills = trainedSkills
                .Select( training => Map( training, skillRepository ) )
                .ToList(),
            Lore = trainedLore
                .Select( training => new CharacterLoreTrainingDto
                {
                    Id = training.LoreId,
                    Name = training.Name,
                    KeyAbility = AbilityType.Intelligence,
                    SourceGrantId = training.SourceGrantId,
                } )
                .ToList(),
            DeferredFeatGrants = featTraining?.DeferredGrants
                .Select( grant => new DeferredFeatTrainingGrantDto
                {
                    FeatId = grant.FeatId,
                    TargetId = grant.TargetId,
                    Reason = grant.Reason.ToString(),
                } )
                .ToArray() ?? [],
        };
    }

    private static CharacterSkillTrainingDto Map(
        TrainedSkill training,
        ISkillRepository? skillRepository )
    {
        SkillDefinition? skill = skillRepository?.GetSkill( training.SkillId );
        return new CharacterSkillTrainingDto
        {
            Id = training.SkillId,
            Name = skill?.Name ?? training.SkillId,
            KeyAbility = skill?.KeyAbility,
            SourceGrantId = training.SourceGrantId,
        };
    }
}
