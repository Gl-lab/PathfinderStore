using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Tests;

public sealed class ClassTrainingResolverTests
{
    [Fact]
    public void ClassSkillGrantDescriptor_NormalizedDuplicateOptions_Throws()
    {
        Assert.Throws<ArgumentException>( () =>
            new ClassSkillGrantDescriptor(
                "class.test.skill.choice",
                [ "skill.arcana", " skill.arcana " ] ) );
    }

    [Fact]
    public void Resolve_AppliesFixedGrantAndAdditionalChoicesIncludingLore()
    {
        CharacterClass characterClass = CreateClass(
            [ new ClassSkillGrantDescriptor( "class.test.skill.arcana", [ "skill.arcana" ] ) ],
            1 );

        ClassTrainingResult result = ClassTrainingResolver.Resolve(
            characterClass,
            [ new ClassSkillGrantChoice( "class.test.skill.arcana", null, null ) ],
            [ new ClassTrainingTargetChoice( null, "Warfare" ) ],
            CreateSkills(),
            0,
            [],
            [] );

        TrainedSkill skill = Assert.Single( result.Skills );
        Assert.Equal( "skill.arcana", skill.SkillId );
        TrainedLore lore = Assert.Single( result.Lore );
        Assert.Equal( "Warfare Lore", lore.Name );
        Assert.Equal( "class.test.skill.additional", lore.SourceGrantId );
    }

    [Fact]
    public void Resolve_UsesFinalIntelligenceModifierForAdditionalChoiceCount()
    {
        CharacterClass characterClass = CreateClass( [], 1 );

        ClassTrainingResult result = ClassTrainingResolver.Resolve(
            characterClass,
            [],
            [
                new ClassTrainingTargetChoice( "skill.arcana", null ),
                new ClassTrainingTargetChoice( "skill.athletics", null ),
            ],
            CreateSkills(),
            1,
            [],
            [] );

        Assert.Equal( 2, result.Skills.Count );
    }

    [Fact]
    public void Resolve_WhenFixedGrantIsAlreadyTrained_RequiresReplacement()
    {
        CharacterClass characterClass = CreateClass(
            [ new ClassSkillGrantDescriptor( "class.test.skill.arcana", [ "skill.arcana" ] ) ],
            0 );

        CharacterManagementException exception = Assert.Throws<CharacterManagementException>( () =>
            ClassTrainingResolver.Resolve(
                characterClass,
                [ new ClassSkillGrantChoice( "class.test.skill.arcana", null, null ) ],
                [],
                CreateSkills(),
                0,
                [ new TrainedSkill( "skill.arcana", "background.test.skill" ) ],
                [] ) );

        Assert.Contains( "requires a replacement", exception.Message );
    }

    [Fact]
    public void Resolve_WhenFixedGrantIsAlreadyTrained_AppliesReplacement()
    {
        CharacterClass characterClass = CreateClass(
            [ new ClassSkillGrantDescriptor( "class.test.skill.arcana", [ "skill.arcana" ] ) ],
            0 );

        ClassTrainingResult result = ClassTrainingResolver.Resolve(
            characterClass,
            [
                new ClassSkillGrantChoice(
                    "class.test.skill.arcana",
                    null,
                    new ClassTrainingTargetChoice( "skill.athletics", null ) ),
            ],
            [],
            CreateSkills(),
            0,
            [ new TrainedSkill( "skill.arcana", "background.test.skill" ) ],
            [] );

        Assert.Contains( result.Skills, skill =>
            ( skill.SkillId == "skill.athletics" ) &&
            ( skill.SourceGrantId == "class.test.skill.arcana" ) );
    }

    [Fact]
    public void Resolve_WhenTargetsRepeat_ThrowsWithoutReturningPartialResult()
    {
        CharacterClass characterClass = CreateClass( [], 2 );

        CharacterManagementException exception = Assert.Throws<CharacterManagementException>( () =>
            ClassTrainingResolver.Resolve(
                characterClass,
                [],
                [
                    new ClassTrainingTargetChoice( "skill.arcana", null ),
                    new ClassTrainingTargetChoice( "skill.arcana", null ),
                ],
                CreateSkills(),
                0,
                [],
                [] ) );

        Assert.Contains( "already trained", exception.Message );
    }

    private static CharacterClass CreateClass(
        IReadOnlyList<ClassSkillGrantDescriptor> initialSkillGrants,
        int additionalSkillCountBase )
    {
        return new CharacterClass(
            "class.test",
            "Test",
            SourceReference.Unknown,
            8,
            [ AbilityType.Intelligence ],
            [
                new ProficiencyGrant(
                    ProficiencyTargets.Perception,
                    ProficiencyRank.Trained,
                    "class.test.initial_proficiencies" ),
            ],
            initialSkillGrants,
            additionalSkillCountBase,
            null,
            [],
            [] );
    }

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        return
        [
            new SkillDefinition(
                "skill.arcana",
                "Arcana",
                AbilityType.Intelligence,
                SourceReference.Unknown ),
            new SkillDefinition(
                "skill.athletics",
                "Athletics",
                AbilityType.Strength,
                SourceReference.Unknown ),
        ];
    }
}
