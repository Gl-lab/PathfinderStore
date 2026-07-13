using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace CharacterManagement.Domain.Tests;

public sealed class BackgroundTrainingTests
{
    [Fact]
    public void SetBackgroundPackage_FixedGrants_AppliesSkillAndLoreTraining()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateFixedBackground();

        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence,
            skillCatalog: CreateSkills() );

        TrainedSkill skill = Assert.Single( character.TrainedSkills );
        Assert.Equal( "skill.acrobatics", skill.SkillId );
        Assert.Equal( "background.test.skill", skill.SourceGrantId );
        TrainedLore lore = Assert.Single( character.TrainedLore );
        Assert.Equal( "lore.circus", lore.LoreId );
        Assert.Equal( "Circus Lore", lore.Name );
        Assert.Equal( "background.test.lore", lore.SourceGrantId );
    }

    [Fact]
    public void SetBackgroundPackage_FiniteChoices_AppliesSelectedTargets()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateChoiceBackground();

        character.SetBackgroundPackage(
            background,
            AbilityType.Wisdom,
            AbilityType.Charisma,
            [
                new BackgroundTrainingChoice(
                    "background.choice.skill",
                    "skill.occultism",
                    null ),
                new BackgroundTrainingChoice(
                    "background.choice.lore",
                    "lore.warfare",
                    null ),
            ],
            CreateSkills() );

        Assert.Equal( "skill.occultism", Assert.Single( character.TrainedSkills ).SkillId );
        TrainedLore lore = Assert.Single( character.TrainedLore );
        Assert.Equal( "lore.warfare", lore.LoreId );
        Assert.Equal( "Warfare Lore", lore.Name );
    }

    [Fact]
    public void SetBackgroundPackage_OpenLore_CreatesCustomTopic()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateOpenLoreBackground();

        character.SetBackgroundPackage(
            background,
            AbilityType.Wisdom,
            AbilityType.Charisma,
            [
                new BackgroundTrainingChoice(
                    "background.open.lore",
                    null,
                    "Ancient Forest" ),
            ],
            CreateSkills() );

        TrainedLore lore = Assert.Single( character.TrainedLore );
        Assert.Equal( "lore.custom.ancient_forest", lore.LoreId );
        Assert.Equal( "Ancient Forest Lore", lore.Name );
    }

    [Fact]
    public void SetBackgroundPackage_InvalidReplacement_PreservesPreviousPackage()
    {
        DraftCharacter character = CreateCharacter();
        Background initial = CreateFixedBackground();
        Background replacement = CreateChoiceBackground();
        character.SetBackgroundPackage(
            initial,
            AbilityType.Dexterity,
            AbilityType.Intelligence,
            skillCatalog: CreateSkills() );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetBackgroundPackage(
                replacement,
                AbilityType.Wisdom,
                AbilityType.Charisma,
                [
                    new BackgroundTrainingChoice(
                        "background.choice.skill",
                        "skill.thievery",
                        null ),
                ],
                CreateSkills() ) );

        Assert.Equal( initial.Id, character.SelectedBackgroundId );
        Assert.Equal( "skill.acrobatics", Assert.Single( character.TrainedSkills ).SkillId );
        Assert.Equal( "lore.circus", Assert.Single( character.TrainedLore ).LoreId );
        Assert.Equal( 12, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 12, character.AbilityScores.Intelligence.Value );
        Assert.Equal( 10, character.AbilityScores.Wisdom.Value );
    }

    [Fact]
    public void SetBackgroundPackage_SamePackageTwice_DoesNotDuplicateTraining()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateFixedBackground();

        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence,
            skillCatalog: CreateSkills() );
        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence,
            skillCatalog: CreateSkills() );

        Assert.Single( character.TrainedSkills );
        Assert.Single( character.TrainedLore );
        Assert.Equal( 12, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 12, character.AbilityScores.Intelligence.Value );
    }

    [Fact]
    public void SetBackgroundPackage_ChoiceForFixedGrant_Throws()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateFixedBackground();

        Assert.Throws<CharacterManagementException>( () =>
            character.SetBackgroundPackage(
                background,
                AbilityType.Dexterity,
                AbilityType.Intelligence,
                [
                    new BackgroundTrainingChoice(
                        "background.test.skill",
                        "skill.acrobatics",
                        null ),
                ],
                CreateSkills() ) );
    }

    [Fact]
    public void SetBackgroundPackage_SkillCatalogMissing_Throws()
    {
        DraftCharacter character = CreateCharacter();

        Assert.Throws<CharacterManagementException>( () =>
            character.SetBackgroundPackage(
                CreateFixedBackground(),
                AbilityType.Dexterity,
                AbilityType.Intelligence ) );
    }

    private static DraftCharacter CreateCharacter()
    {
        return DraftCharacter.Create( 1, "Tester", AncestryType.Human );
    }

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        return
        [
            new SkillDefinition( "skill.acrobatics", "Acrobatics", AbilityType.Dexterity, SourceReference.Unknown ),
            new SkillDefinition( "skill.nature", "Nature", AbilityType.Wisdom, SourceReference.Unknown ),
            new SkillDefinition( "skill.occultism", "Occultism", AbilityType.Intelligence, SourceReference.Unknown ),
        ];
    }

    private static Background CreateFixedBackground()
    {
        return CreateBackground(
            "background.test",
            FixedGrant(
                "background.test.skill",
                BackgroundGrantKind.SkillTraining,
                "skill.acrobatics",
                "Acrobatics" ),
            FixedGrant(
                "background.test.lore",
                BackgroundGrantKind.LoreTraining,
                "lore.circus",
                "Circus Lore" ) );
    }

    private static Background CreateChoiceBackground()
    {
        return CreateBackground(
            "background.choice",
            ChoiceGrant(
                "background.choice.skill",
                BackgroundGrantKind.SkillTraining,
                [
                    new BackgroundGrantOption( "skill.nature", "Nature" ),
                    new BackgroundGrantOption( "skill.occultism", "Occultism" ),
                ] ),
            ChoiceGrant(
                "background.choice.lore",
                BackgroundGrantKind.LoreTraining,
                [
                    new BackgroundGrantOption( "lore.legal", "Legal Lore" ),
                    new BackgroundGrantOption( "lore.warfare", "Warfare Lore" ),
                ] ) );
    }

    private static Background CreateOpenLoreBackground()
    {
        return CreateBackground(
            "background.open",
            FixedGrant(
                "background.open.skill",
                BackgroundGrantKind.SkillTraining,
                "skill.nature",
                "Nature" ),
            new BackgroundGrantDescriptor(
                "background.open.lore",
                BackgroundGrantKind.LoreTraining,
                "Terrain Lore",
                "Choose terrain Lore.",
                true,
                true,
                null,
                [],
                [] ) );
    }

    private static Background CreateBackground(
        string id,
        BackgroundGrantDescriptor skillGrant,
        BackgroundGrantDescriptor loreGrant )
    {
        return new Background(
            id,
            id,
            SourceReference.Unknown,
            [ AbilityType.Dexterity, AbilityType.Wisdom ],
            1,
            [
                skillGrant,
                loreGrant,
                FixedGrant(
                    $"{id}.skill_feat",
                    BackgroundGrantKind.SkillFeat,
                    "skill_feat.test",
                    "Test Feat" ),
            ] );
    }

    private static BackgroundGrantDescriptor FixedGrant(
        string id,
        BackgroundGrantKind kind,
        string targetId,
        string name )
    {
        return new BackgroundGrantDescriptor(
            id,
            kind,
            name,
            name,
            false,
            false,
            targetId,
            [],
            [] );
    }

    private static BackgroundGrantDescriptor ChoiceGrant(
        string id,
        BackgroundGrantKind kind,
        IReadOnlyList<BackgroundGrantOption> options )
    {
        return new BackgroundGrantDescriptor(
            id,
            kind,
            id,
            id,
            true,
            false,
            null,
            options,
            [] );
    }
}
