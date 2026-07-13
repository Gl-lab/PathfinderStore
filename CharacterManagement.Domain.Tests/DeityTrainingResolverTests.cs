using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace CharacterManagement.Domain.Tests;

public sealed class DeityTrainingResolverTests
{
    [Fact]
    public void Resolve_UntrainedDivineSkill_AddsTraining()
    {
        Deity deity = CreateDeity();

        IReadOnlyList<TrainedSkill> result = DeityTrainingResolver.Resolve(
            deity,
            null,
            CreateSkills(),
            [] );

        TrainedSkill training = Assert.Single( result );
        Assert.Equal( "skill.medicine", training.SkillId );
        Assert.Equal( "deity.test.skill.divine", training.SourceGrantId );
    }

    [Fact]
    public void Resolve_AlreadyTrainedDivineSkill_RequiresUnusedReplacement()
    {
        Deity deity = CreateDeity();
        TrainedSkill existing = new TrainedSkill( "skill.medicine", "background.test.skill" );

        Assert.Throws<CharacterManagementException>( () => DeityTrainingResolver.Resolve(
            deity,
            null,
            CreateSkills(),
            [ existing ] ) );

        IReadOnlyList<TrainedSkill> result = DeityTrainingResolver.Resolve(
            deity,
            "skill.arcana",
            CreateSkills(),
            [ existing ] );

        Assert.Contains( result, training => training.SkillId == "skill.medicine" );
        Assert.Contains( result, training => training.SkillId == "skill.arcana" );
    }

    [Fact]
    public void ProficiencyResolver_CombinesClericDoctrineAndDeityWeapon()
    {
        Deity deity = CreateDeity();
        IReadOnlyList<EffectiveProficiency> result = ProficiencyResolver.Resolve(
            deity.ProficiencyGrants.Concat(
            [
                new ProficiencyGrant(
                    ProficiencyTargets.Fortitude,
                    ProficiencyRank.Expert,
                    "cleric_doctrine.warpriest.proficiency.fortitude" ),
            ] ) );

        Assert.Contains(
            result,
            proficiency => proficiency.Target.Id == "proficiency.attack.weapon.longsword" );
        Assert.Contains(
            result,
            proficiency => proficiency.Target.Id == ProficiencyTargets.Fortitude.Id &&
                           proficiency.Rank == ProficiencyRank.Expert );
    }

    private static Deity CreateDeity()
    {
        return new Deity(
            "deity.test",
            "Test",
            new SourceReference( "Player Core", 35 ),
            true,
            "skill.medicine",
            [ new DeityFavoredWeapon( "weapon.longsword", "Longsword", FavoredWeaponCategory.Martial ) ],
            [ DivineFont.Heal ],
            [],
            null,
            [ "domain.healing" ],
            [] );
    }

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        SourceReference source = new SourceReference( "Player Core", 227 );
        return
        [
            new SkillDefinition( "skill.medicine", "Medicine", AbilityType.Wisdom, source ),
            new SkillDefinition( "skill.arcana", "Arcana", AbilityType.Intelligence, source ),
        ];
    }
}
