using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace CharacterManagement.Domain.Tests;

public sealed class FeatTrainingResolverTests
{
    [Fact]
    public void Resolve_FixedLoreFeat_AddsSkillsLoreAndProvenance()
    {
        CharacterFeat feat = CharacterFeat( "dwarf.dwarven_lore", FeatCategory.Ancestry, "Dwarf" );

        FeatTrainingResult result = FeatTrainingResolver.Resolve( [ feat ], [], [] );

        Assert.Equal( [ "skill.crafting", "skill.religion" ], result.Skills.Select( skill => skill.SkillId ) );
        Assert.All( result.Skills, skill => Assert.Equal( feat.Definition.Id, skill.SourceGrantId ) );
        TrainedLore lore = Assert.Single( result.Lore );
        Assert.Equal( "lore.dwarf", lore.LoreId );
        Assert.Equal( "Dwarf Lore", lore.Name );
        Assert.Equal( feat.Definition.Id, lore.SourceGrantId );
        Assert.Empty( result.DeferredGrants );
    }

    [Fact]
    public void Resolve_ExistingTraining_DefersReplacementWithoutDuplicatingSkill()
    {
        CharacterFeat feat = CharacterFeat( "halfling.prairie_rider", FeatCategory.Ancestry, "Halfling" );
        TrainedSkill existing = new TrainedSkill( "skill.nature", "background.herbalist.skill" );

        FeatTrainingResult result = FeatTrainingResolver.Resolve( [ feat ], [ existing ], [] );

        Assert.Same( existing, Assert.Single( result.Skills ) );
        DeferredFeatTrainingGrant deferred = Assert.Single( result.DeferredGrants );
        Assert.Equal( feat.Definition.Id, deferred.FeatId );
        Assert.Equal( "skill.nature", deferred.TargetId );
        Assert.Equal( DeferredFeatTrainingReason.ReplacementChoiceRequired, deferred.Reason );
    }

    [Fact]
    public void Resolve_BardicLore_AddsSpecialLoreTraining()
    {
        CharacterFeat feat = CharacterFeat( "feat.bardic_lore", FeatCategory.Class, "Bard" );

        FeatTrainingResult result = FeatTrainingResolver.Resolve( [ feat ], [], [] );

        TrainedLore lore = Assert.Single( result.Lore );
        Assert.Equal( "lore.bardic", lore.LoreId );
        Assert.Equal( feat.Definition.Id, lore.SourceGrantId );
    }

    [Fact]
    public void EffectiveFeatTraining_ChangesSkillModifierAndKeepsFeatSource()
    {
        CharacterFeat feat = CharacterFeat( "halfling.prairie_rider", FeatCategory.Ancestry, "Halfling" );
        FeatTrainingResult training = FeatTrainingResolver.Resolve( [ feat ], [], [] );
        DraftCharacter character = DraftCharacter.Create( 1, "Lem", AncestryType.Halfling );

        CharacterSkillModifiersDto modifiers = CharacterSkillModifiersDtoMapper.Map(
            character,
            new NatureSkillRepository(),
            1,
            training.Skills,
            training.Lore );

        CharacterProficiencyStatisticDto nature = Assert.Single( modifiers.General );
        Assert.Equal( ProficiencyRank.Trained, nature.ProficiencyRank );
        Assert.Equal( 3, nature.Total );
        Assert.Equal( [ feat.Definition.Id ], nature.SourceGrantIds );
    }

    private static CharacterFeat CharacterFeat(
        string id,
        FeatCategory category,
        string trait )
    {
        FeatDefinition definition = new FeatDefinition(
            id,
            id,
            category,
            1,
            [ trait ],
            FeatRarity.Common,
            [],
            "Test feat.",
            [ FeatDependencyType.RuleEngine ],
            SourceReference.Unknown );
        return new CharacterFeat(
            definition,
            CharacterFeatAcquisitionType.Selected,
            CharacterFeatSourceType.Ancestry,
            "test" );
    }

    private sealed class NatureSkillRepository : ISkillRepository
    {
        private static readonly SkillDefinition Nature = new SkillDefinition(
            "skill.nature",
            "Nature",
            AbilityType.Wisdom,
            SourceReference.Unknown );

        public IReadOnlyCollection<SkillDefinition> GetAll() => [ Nature ];

        public SkillDefinition GetSkill( string skillId ) => Nature;
    }
}
