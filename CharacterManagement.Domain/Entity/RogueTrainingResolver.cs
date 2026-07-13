using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public static class RogueTrainingResolver
{
    private const string StealthGrantId = "class.rogue.skill.stealth";
    private const string StealthSkillId = "skill.stealth";

    public static RogueTrainingResult Resolve(
        RogueRacket racket,
        IReadOnlyList<RogueTrainingChoice> choices,
        IReadOnlyCollection<SkillDefinition> generalSkills,
        IReadOnlyCollection<TrainedSkill> existingTraining )
    {
        ArgumentNullException.ThrowIfNull( racket );
        ArgumentNullException.ThrowIfNull( choices );
        ArgumentNullException.ThrowIfNull( generalSkills );
        ArgumentNullException.ThrowIfNull( existingTraining );

        if ( choices.Select( choice => choice.GrantId ).Distinct( StringComparer.Ordinal ).Count() != choices.Count )
        {
            throw new CharacterManagementException( "Rogue training choices must use unique grant ids." );
        }

        if ( existingTraining.Select( training => training.SkillId ).Distinct( StringComparer.Ordinal ).Count() != existingTraining.Count )
        {
            throw new CharacterManagementException( "Existing skill training must not contain duplicate skills." );
        }

        List<RogueSkillGrantDescriptor> grants =
        [
            new RogueSkillGrantDescriptor( StealthGrantId, StealthSkillId, [] ),
            .. racket.SkillGrants,
        ];
        HashSet<string> grantIds = grants
            .Select( grant => grant.Id )
            .ToHashSet( StringComparer.Ordinal );

        if ( choices.Any( choice => !grantIds.Contains( choice.GrantId ) ) )
        {
            throw new CharacterManagementException( "Rogue training choice does not belong to the selected racket package." );
        }

        Dictionary<string, SkillDefinition> skillCatalog = generalSkills
            .ToDictionary( skill => skill.Id, StringComparer.Ordinal );
        List<TrainedSkill> resolved = existingTraining.ToList();
        HashSet<string> trainedSkillIds = resolved
            .Select( training => training.SkillId )
            .ToHashSet( StringComparer.Ordinal );

        foreach ( RogueSkillGrantDescriptor grant in grants )
        {
            RogueTrainingChoice? choice = choices
                .SingleOrDefault( item => item.GrantId == grant.Id );
            string targetId = ResolveTarget( grant, choice );
            ValidateCatalogSkill( targetId, skillCatalog );

            bool hasConflict = trainedSkillIds.Contains( targetId );
            string effectiveSkillId = ResolveEffectiveSkill(
                grant,
                choice,
                targetId,
                hasConflict,
                skillCatalog,
                trainedSkillIds );

            resolved.Add( new TrainedSkill( effectiveSkillId, grant.Id ) );
            trainedSkillIds.Add( effectiveSkillId );
        }

        return new RogueTrainingResult( resolved );
    }

    private static string ResolveTarget(
        RogueSkillGrantDescriptor grant,
        RogueTrainingChoice? choice )
    {
        if ( !grant.RequiresChoice )
        {
            if ( ( choice is not null ) && !String.IsNullOrWhiteSpace( choice.SelectedSkillId ) )
            {
                throw new CharacterManagementException(
                    $"Fixed Rogue grant '{grant.Id}' does not accept a selected skill." );
            }

            return grant.TargetId!;
        }

        if ( ( choice is null ) || String.IsNullOrWhiteSpace( choice.SelectedSkillId ) )
        {
            throw new CharacterManagementException(
                $"Rogue grant '{grant.Id}' requires one catalog option." );
        }

        if ( !grant.Options.Contains( choice.SelectedSkillId, StringComparer.Ordinal ) )
        {
            throw new CharacterManagementException(
                $"Skill '{choice.SelectedSkillId}' is not valid for Rogue grant '{grant.Id}'." );
        }

        return choice.SelectedSkillId;
    }

    private static string ResolveEffectiveSkill(
        RogueSkillGrantDescriptor grant,
        RogueTrainingChoice? choice,
        string targetId,
        bool hasConflict,
        IReadOnlyDictionary<string, SkillDefinition> skillCatalog,
        IReadOnlySet<string> trainedSkillIds )
    {
        string? replacementId = choice?.ReplacementSkillId;
        if ( !hasConflict )
        {
            if ( !String.IsNullOrWhiteSpace( replacementId ) )
            {
                throw new CharacterManagementException(
                    $"Rogue grant '{grant.Id}' cannot replace '{targetId}' because it is not already trained." );
            }

            return targetId;
        }

        if ( String.IsNullOrWhiteSpace( replacementId ) )
        {
            throw new CharacterManagementException(
                $"Rogue grant '{grant.Id}' requires a replacement because '{targetId}' is already trained." );
        }

        ValidateCatalogSkill( replacementId, skillCatalog );
        if ( trainedSkillIds.Contains( replacementId ) )
        {
            throw new CharacterManagementException(
                $"Replacement skill '{replacementId}' is already trained." );
        }

        return replacementId;
    }

    private static void ValidateCatalogSkill(
        string skillId,
        IReadOnlyDictionary<string, SkillDefinition> skillCatalog )
    {
        if ( !skillCatalog.ContainsKey( skillId ) )
        {
            throw new CharacterManagementException( $"Skill '{skillId}' is not defined in the skill catalog." );
        }
    }
}
