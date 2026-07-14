using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public static class ClassTrainingResolver
{
    public static ClassTrainingResult Resolve(
        CharacterClass characterClass,
        IReadOnlyList<ClassSkillGrantChoice> grantChoices,
        IReadOnlyList<ClassTrainingTargetChoice> additionalChoices,
        IReadOnlyCollection<SkillDefinition> generalSkills,
        int intelligenceModifier,
        IReadOnlyCollection<TrainedSkill> existingSkills,
        IReadOnlyCollection<TrainedLore> existingLore )
    {
        ArgumentNullException.ThrowIfNull( characterClass );
        ArgumentNullException.ThrowIfNull( grantChoices );
        ArgumentNullException.ThrowIfNull( additionalChoices );
        ArgumentNullException.ThrowIfNull( generalSkills );
        ArgumentNullException.ThrowIfNull( existingSkills );
        ArgumentNullException.ThrowIfNull( existingLore );

        if ( generalSkills.Count == 0 )
        {
            throw new CharacterManagementException( "Skill catalog is required to resolve class training." );
        }

        if ( generalSkills
            .Select( skill => skill.Id )
            .Distinct( StringComparer.Ordinal )
            .Count() != generalSkills.Count )
        {
            throw new CharacterManagementException( "Skill catalog ids must be unique." );
        }

        if ( grantChoices
            .Select( choice => choice.GrantId )
            .Distinct( StringComparer.Ordinal )
            .Count() != grantChoices.Count )
        {
            throw new CharacterManagementException( "Class skill grant choices must use unique grant ids." );
        }

        HashSet<string> expectedGrantIds = characterClass.InitialSkillGrants
            .Select( grant => grant.Id )
            .ToHashSet( StringComparer.Ordinal );
        HashSet<string> actualGrantIds = grantChoices
            .Select( choice => choice.GrantId )
            .ToHashSet( StringComparer.Ordinal );
        if ( !expectedGrantIds.SetEquals( actualGrantIds ) )
        {
            throw new CharacterManagementException(
                "Class skill grant choices must match the selected class grants." );
        }

        int requiredAdditionalCount = characterClass.AdditionalSkillCountBase + intelligenceModifier;
        if ( requiredAdditionalCount < 0 )
        {
            throw new CharacterManagementException( "Class additional skill count cannot be negative." );
        }

        if ( additionalChoices.Count != requiredAdditionalCount )
        {
            throw new CharacterManagementException(
                $"Class requires exactly {requiredAdditionalCount} additional skill choices." );
        }

        List<TrainedSkill> trainedSkills = existingSkills.ToList();
        List<TrainedLore> trainedLore = existingLore.ToList();
        HashSet<string> trainedTargetIds = existingSkills
            .Select( training => training.SkillId )
            .Concat( existingLore.Select( training => training.LoreId ) )
            .ToHashSet( StringComparer.Ordinal );

        foreach ( ClassSkillGrantDescriptor grant in characterClass.InitialSkillGrants )
        {
            ClassSkillGrantChoice choice = grantChoices.Single( item => item.GrantId == grant.Id );
            string initialSkillId = ResolveInitialSkillId( grant, choice, generalSkills );
            bool requiresReplacement = trainedTargetIds.Contains( initialSkillId );

            if ( requiresReplacement )
            {
                if ( choice.ReplacementTarget is null )
                {
                    throw new CharacterManagementException(
                        $"Class skill grant '{grant.Id}' requires a replacement target." );
                }

                AddTarget(
                    choice.ReplacementTarget,
                    grant.Id,
                    generalSkills,
                    trainedTargetIds,
                    trainedSkills,
                    trainedLore );
            }
            else
            {
                if ( choice.ReplacementTarget is not null )
                {
                    throw new CharacterManagementException(
                        $"Class skill grant '{grant.Id}' does not accept a replacement target." );
                }

                trainedSkills.Add( new TrainedSkill( initialSkillId, grant.Id ) );
                trainedTargetIds.Add( initialSkillId );
            }
        }

        string additionalSourceGrantId = $"{characterClass.Id}.skill.additional";
        foreach ( ClassTrainingTargetChoice additionalChoice in additionalChoices )
        {
            AddTarget(
                additionalChoice,
                additionalSourceGrantId,
                generalSkills,
                trainedTargetIds,
                trainedSkills,
                trainedLore );
        }

        return new ClassTrainingResult(
            trainedSkills.ToArray(),
            trainedLore.ToArray() );
    }

    private static string ResolveInitialSkillId(
        ClassSkillGrantDescriptor grant,
        ClassSkillGrantChoice choice,
        IReadOnlyCollection<SkillDefinition> generalSkills )
    {
        string skillId;
        if ( grant.SkillOptions.Count == 1 )
        {
            if ( !String.IsNullOrWhiteSpace( choice.SelectedSkillId ) )
            {
                throw new CharacterManagementException(
                    $"Fixed class skill grant '{grant.Id}' does not accept a selected skill." );
            }

            skillId = grant.SkillOptions[ 0 ];
        }
        else
        {
            if ( String.IsNullOrWhiteSpace( choice.SelectedSkillId ) ||
                 !grant.SkillOptions.Contains( choice.SelectedSkillId, StringComparer.Ordinal ) )
            {
                throw new CharacterManagementException(
                    $"Class skill grant '{grant.Id}' requires one catalog option." );
            }

            skillId = choice.SelectedSkillId;
        }

        if ( !generalSkills.Any( skill => skill.Id == skillId ) )
        {
            throw new CharacterManagementException( $"Skill '{skillId}' is not defined in the skill catalog." );
        }

        return skillId;
    }

    private static void AddTarget(
        ClassTrainingTargetChoice choice,
        string sourceGrantId,
        IReadOnlyCollection<SkillDefinition> generalSkills,
        ISet<string> trainedTargetIds,
        ICollection<TrainedSkill> trainedSkills,
        ICollection<TrainedLore> trainedLore )
    {
        bool hasSkill = !String.IsNullOrWhiteSpace( choice.SkillId );
        bool hasLore = !String.IsNullOrWhiteSpace( choice.CustomLoreTopic );
        if ( hasSkill == hasLore )
        {
            throw new CharacterManagementException(
                "Class training target must define exactly one general skill or custom Lore topic." );
        }

        if ( hasSkill )
        {
            string skillId = choice.SkillId!.Trim();
            if ( !generalSkills.Any( skill => skill.Id == skillId ) )
            {
                throw new CharacterManagementException( $"Skill '{skillId}' is not defined in the skill catalog." );
            }

            if ( !trainedTargetIds.Add( skillId ) )
            {
                throw new CharacterManagementException( $"Skill '{skillId}' is already trained." );
            }

            trainedSkills.Add( new TrainedSkill( skillId, sourceGrantId ) );
            return;
        }

        LoreTopic loreTopic = LoreTopic.CreateCustom( choice.CustomLoreTopic!, generalSkills );
        if ( !trainedTargetIds.Add( loreTopic.Id ) )
        {
            throw new CharacterManagementException( $"Lore '{loreTopic.Name}' is already trained." );
        }

        trainedLore.Add( new TrainedLore( loreTopic.Id, loreTopic.Name, sourceGrantId ) );
    }
}
