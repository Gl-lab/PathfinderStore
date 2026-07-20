using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace Pathfinder.CharacterManagement.Domain.Rules.Feats;

public enum DeferredFeatTrainingReason
{
    ReplacementChoiceRequired
}

public sealed record DeferredFeatTrainingGrant(
    string FeatId,
    string TargetId,
    DeferredFeatTrainingReason Reason );

public sealed record FeatTrainingResult(
    IReadOnlyList<TrainedSkill> Skills,
    IReadOnlyList<TrainedLore> Lore,
    IReadOnlyList<DeferredFeatTrainingGrant> DeferredGrants );

public static class FeatTrainingResolver
{
    private static readonly IReadOnlyDictionary<string, FeatTrainingDefinition> Definitions =
        new Dictionary<string, FeatTrainingDefinition>( StringComparer.Ordinal )
        {
            [ "dwarf.dwarven_lore" ] = new(
                [ "skill.crafting", "skill.religion" ],
                [ new LoreGrant( "lore.dwarf", "Dwarf Lore" ) ] ),
            [ "elf.elven_lore" ] = new(
                [ "skill.arcana", "skill.nature" ],
                [ new LoreGrant( "lore.elf", "Elf Lore" ) ] ),
            [ "goblin.goblin_lore" ] = new(
                [ "skill.nature", "skill.stealth" ],
                [ new LoreGrant( "lore.goblin", "Goblin Lore" ) ] ),
            [ "halfling.halfling_lore" ] = new(
                [ "skill.acrobatics", "skill.stealth" ],
                [ new LoreGrant( "lore.halfling", "Halfling Lore" ) ] ),
            [ "halfling.prairie_rider" ] = new(
                [ "skill.nature" ],
                [] ),
            [ "feat.bardic_lore" ] = new(
                [],
                [ new LoreGrant( "lore.bardic", "Bardic Lore" ) ] ),
        };

    public static FeatTrainingResult Resolve(
        IReadOnlyCollection<CharacterFeat> feats,
        IReadOnlyList<TrainedSkill> existingSkills,
        IReadOnlyList<TrainedLore> existingLore )
    {
        ArgumentNullException.ThrowIfNull( feats );
        ArgumentNullException.ThrowIfNull( existingSkills );
        ArgumentNullException.ThrowIfNull( existingLore );

        List<TrainedSkill> skills = existingSkills.ToList();
        List<TrainedLore> lore = existingLore.ToList();
        List<DeferredFeatTrainingGrant> deferred = [];

        foreach ( CharacterFeat feat in feats )
        {
            if ( !Definitions.TryGetValue( feat.Definition.Id, out FeatTrainingDefinition? definition ) )
            {
                continue;
            }

            foreach ( string skillId in definition.SkillIds )
            {
                if ( skills.Any( training => training.SkillId == skillId ) )
                {
                    deferred.Add( new DeferredFeatTrainingGrant(
                        feat.Definition.Id,
                        skillId,
                        DeferredFeatTrainingReason.ReplacementChoiceRequired ) );
                }
                else
                {
                    skills.Add( new TrainedSkill( skillId, feat.Definition.Id ) );
                }
            }

            foreach ( LoreGrant loreGrant in definition.Lore )
            {
                if ( lore.Any( training => training.LoreId == loreGrant.Id ) )
                {
                    deferred.Add( new DeferredFeatTrainingGrant(
                        feat.Definition.Id,
                        loreGrant.Id,
                        DeferredFeatTrainingReason.ReplacementChoiceRequired ) );
                }
                else
                {
                    lore.Add( new TrainedLore( loreGrant.Id, loreGrant.Name, feat.Definition.Id ) );
                }
            }
        }

        return new FeatTrainingResult(
            skills.OrderBy( training => training.SkillId, StringComparer.Ordinal ).ToArray(),
            lore.OrderBy( training => training.Name, StringComparer.Ordinal ).ToArray(),
            deferred
                .OrderBy( grant => grant.FeatId, StringComparer.Ordinal )
                .ThenBy( grant => grant.TargetId, StringComparer.Ordinal )
                .ToArray() );
    }

    private sealed record FeatTrainingDefinition(
        IReadOnlyList<string> SkillIds,
        IReadOnlyList<LoreGrant> Lore );

    private sealed record LoreGrant( string Id, string Name );
}
