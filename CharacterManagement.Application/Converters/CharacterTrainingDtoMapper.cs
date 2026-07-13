using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterTrainingDtoMapper
{
    public static CharacterTrainingDto Map(
        DraftCharacter character,
        ISkillRepository? skillRepository )
    {
        ArgumentNullException.ThrowIfNull( character );

        if ( ( character.TrainedSkills.Count > 0 ) && ( skillRepository is null ) )
        {
            throw new InvalidOperationException( "Skill repository is required to map trained skills." );
        }

        return new CharacterTrainingDto
        {
            Skills = character.TrainedSkills
                .Select( training => Map( training, skillRepository ) )
                .ToList(),
            Lore = character.TrainedLore
                .Select( training => new CharacterLoreTrainingDto
                {
                    Id = training.LoreId,
                    Name = training.Name,
                    KeyAbility = AbilityType.Intelligence,
                    SourceGrantId = training.SourceGrantId,
                } )
                .ToList(),
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
