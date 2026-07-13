using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public static class DeityTrainingResolver
{
    public static IReadOnlyList<TrainedSkill> Resolve(
        Deity deity,
        string? replacementSkillId,
        IReadOnlyCollection<SkillDefinition> generalSkills,
        IReadOnlyCollection<TrainedSkill> existingTraining )
    {
        ArgumentNullException.ThrowIfNull( deity );
        ArgumentNullException.ThrowIfNull( generalSkills );
        ArgumentNullException.ThrowIfNull( existingTraining );

        if ( !deity.CanGrantClericPowers || String.IsNullOrWhiteSpace( deity.DivineSkillId ) )
        {
            throw new CharacterManagementException( $"Deity '{deity.Id}' cannot grant Cleric training." );
        }

        Dictionary<string, SkillDefinition> catalog = generalSkills
            .ToDictionary( skill => skill.Id, StringComparer.Ordinal );
        if ( !catalog.ContainsKey( deity.DivineSkillId ) )
        {
            throw new CharacterManagementException(
                $"Divine skill '{deity.DivineSkillId}' is not defined in the skill catalog." );
        }

        HashSet<string> trainedIds = existingTraining
            .Select( training => training.SkillId )
            .ToHashSet( StringComparer.Ordinal );
        bool hasConflict = trainedIds.Contains( deity.DivineSkillId );
        string effectiveSkillId;

        if ( hasConflict )
        {
            if ( String.IsNullOrWhiteSpace( replacementSkillId ) )
            {
                throw new CharacterManagementException(
                    $"Deity '{deity.Id}' requires a replacement because '{deity.DivineSkillId}' is already trained." );
            }

            if ( !catalog.ContainsKey( replacementSkillId ) )
            {
                throw new CharacterManagementException(
                    $"Replacement skill '{replacementSkillId}' is not defined in the skill catalog." );
            }

            if ( trainedIds.Contains( replacementSkillId ) )
            {
                throw new CharacterManagementException(
                    $"Replacement skill '{replacementSkillId}' is already trained." );
            }

            effectiveSkillId = replacementSkillId;
        }
        else
        {
            if ( !String.IsNullOrWhiteSpace( replacementSkillId ) )
            {
                throw new CharacterManagementException(
                    $"Deity '{deity.Id}' cannot replace an untrained divine skill." );
            }

            effectiveSkillId = deity.DivineSkillId;
        }

        return
        [
            .. existingTraining,
            new TrainedSkill( effectiveSkillId, deity.DivineSkillGrantId ),
        ];
    }
}
