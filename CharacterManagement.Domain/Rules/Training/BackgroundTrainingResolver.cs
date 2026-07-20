using Pathfinder.CharacterManagement.Domain.Exceptions;

using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Training;

public static class BackgroundTrainingResolver
{
    public static BackgroundTrainingResult Resolve(
        Background background,
        IReadOnlyList<BackgroundTrainingChoice> choices,
        IReadOnlyCollection<SkillDefinition> generalSkills )
    {
        ArgumentNullException.ThrowIfNull( background );
        ArgumentNullException.ThrowIfNull( choices );
        ArgumentNullException.ThrowIfNull( generalSkills );

        if ( choices.Select( choice => choice.GrantId ).Distinct( StringComparer.Ordinal ).Count() != choices.Count )
        {
            throw new CharacterManagementException( "Background training choices must use unique grant ids." );
        }

        IReadOnlyList<BackgroundGrantDescriptor> trainingGrants = background.Grants
            .Where( grant =>
                grant.Kind == BackgroundGrantKind.SkillTraining ||
                grant.Kind == BackgroundGrantKind.LoreTraining ||
                grant.Kind == BackgroundGrantKind.SkillFeat )
            .ToList();
        if ( trainingGrants.Any( grant => grant.Kind == BackgroundGrantKind.SkillTraining ) &&
             generalSkills.Count == 0 )
        {
            throw new CharacterManagementException( "Skill catalog is required to resolve background training." );
        }
        HashSet<string> choiceGrantIds = trainingGrants
            .Where( grant => grant.RequiresChoice )
            .Select( grant => grant.Id )
            .ToHashSet( StringComparer.Ordinal );

        if ( choices.Any( choice => !choiceGrantIds.Contains( choice.GrantId ) ) )
        {
            throw new CharacterManagementException( "Background training choice does not belong to the selected background." );
        }

        List<TrainedSkill> trainedSkills = [];
        List<TrainedLore> trainedLore = [];
        string? skillFeatId = null;

        foreach ( BackgroundGrantDescriptor grant in trainingGrants )
        {
            BackgroundTrainingChoice? choice = choices
                .SingleOrDefault( item => item.GrantId == grant.Id );

            if ( grant.Kind == BackgroundGrantKind.SkillTraining )
            {
                trainedSkills.Add( ResolveSkill( grant, choice, generalSkills ) );
            }
            else if ( grant.Kind == BackgroundGrantKind.LoreTraining )
            {
                trainedLore.Add( ResolveLore( grant, choice, generalSkills ) );
            }
            else
            {
                skillFeatId = ResolveTargetId( grant, choice );
            }
        }

        if ( trainedSkills.Select( skill => skill.SkillId ).Distinct( StringComparer.Ordinal ).Count() != trainedSkills.Count )
        {
            throw new CharacterManagementException( "Background cannot grant the same skill training more than once." );
        }

        if ( trainedLore.Select( lore => lore.LoreId ).Distinct( StringComparer.Ordinal ).Count() != trainedLore.Count )
        {
            throw new CharacterManagementException( "Background cannot grant the same Lore training more than once." );
        }

        if ( trainingGrants.Any( grant => grant.Kind == BackgroundGrantKind.SkillFeat ) &&
             String.IsNullOrWhiteSpace( skillFeatId ) )
        {
            throw new CharacterManagementException( "Background must grant exactly one skill feat." );
        }

        return new BackgroundTrainingResult( trainedSkills, trainedLore, skillFeatId );
    }

    private static TrainedSkill ResolveSkill(
        BackgroundGrantDescriptor grant,
        BackgroundTrainingChoice? choice,
        IReadOnlyCollection<SkillDefinition> generalSkills )
    {
        string skillId = ResolveTargetId( grant, choice );
        if ( !generalSkills.Any( skill => skill.Id == skillId ) )
        {
            throw new CharacterManagementException( $"Skill '{skillId}' is not defined in the skill catalog." );
        }

        return new TrainedSkill( skillId, grant.Id );
    }

    private static TrainedLore ResolveLore(
        BackgroundGrantDescriptor grant,
        BackgroundTrainingChoice? choice,
        IReadOnlyCollection<SkillDefinition> generalSkills )
    {
        LoreTopic loreTopic;
        if ( grant.AllowsCustomLore )
        {
            if ( choice is null ||
                 !String.IsNullOrWhiteSpace( choice.TargetId ) ||
                 String.IsNullOrWhiteSpace( choice.CustomLoreTopic ) )
            {
                throw new CharacterManagementException(
                    $"Background grant '{grant.Id}' requires a custom Lore topic." );
            }

            loreTopic = LoreTopic.CreateCustom( choice.CustomLoreTopic, generalSkills );
        }
        else
        {
            string loreId = ResolveTargetId( grant, choice );
            string loreName = grant.RequiresChoice
                ? grant.Options.Single( option => option.Id == loreId ).Name
                : grant.Name;
            loreTopic = LoreTopic.CreateKnown( loreId, loreName );
        }

        return new TrainedLore( loreTopic.Id, loreTopic.Name, grant.Id );
    }

    private static string ResolveTargetId(
        BackgroundGrantDescriptor grant,
        BackgroundTrainingChoice? choice )
    {
        if ( !grant.RequiresChoice )
        {
            if ( choice is not null )
            {
                throw new CharacterManagementException(
                    $"Fixed background grant '{grant.Id}' does not accept a choice." );
            }

            return grant.TargetId!;
        }

        if ( choice is null ||
             String.IsNullOrWhiteSpace( choice.TargetId ) ||
             !String.IsNullOrWhiteSpace( choice.CustomLoreTopic ) )
        {
            throw new CharacterManagementException(
                $"Background grant '{grant.Id}' requires one catalog option." );
        }

        if ( !grant.Options.Any( option => option.Id == choice.TargetId ) )
        {
            throw new CharacterManagementException(
                $"Option '{choice.TargetId}' is not valid for background grant '{grant.Id}'." );
        }

        return choice.TargetId;
    }
}
