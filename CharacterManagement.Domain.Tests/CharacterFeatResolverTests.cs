using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;
using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace CharacterManagement.Domain.Tests;

public sealed class CharacterFeatResolverTests
{
    [Fact]
    public void ResolveAncestryAndBackground_ReturnsSelectedAndGrantedProvenance()
    {
        Ancestry ancestry = CreateAncestry();
        Background background = CreateBackground();
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Human );
        character.SetAncestryPackage(
            null,
            ancestry,
            "human.skilled",
            "human.cooperative_nature",
            new CommonAncestryChoiceAvailabilityPolicy() );
        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence,
            skillCatalog: CreateSkills() );

        IReadOnlyCollection<CharacterFeat> result = CharacterFeatResolver.Resolve(
            character,
            background,
            null,
            null,
            null,
            null,
            null,
            null,
            [
                Feat( "human.cooperative_nature", "Cooperative Nature", FeatCategory.Ancestry, "Human" ),
                Feat( "skill_feat.steady_balance", "Steady Balance", FeatCategory.Skill, "Skill" ),
            ] );

        CharacterFeat selected = Assert.Single(
            result,
            feat => feat.AcquisitionType == CharacterFeatAcquisitionType.Selected );
        Assert.Equal( "human.cooperative_nature", selected.Definition.Id );
        Assert.Equal( CharacterFeatSourceType.Ancestry, selected.SourceType );
        Assert.Equal( "ancestry.human", selected.SourceId );

        CharacterFeat granted = Assert.Single(
            result,
            feat => feat.AcquisitionType == CharacterFeatAcquisitionType.Granted );
        Assert.Equal( "skill_feat.steady_balance", granted.Definition.Id );
        Assert.Equal( CharacterFeatSourceType.Background, granted.SourceType );
        Assert.Equal( background.Id, granted.SourceId );
    }

    [Fact]
    public void ResolveClassChoices_RequiresClassPackageSlotAndClassTrait()
    {
        CharacterClass fighter = CreateClass(
            "fighter",
            "Fighter",
            [ new CharacterClassRuleDescriptor(
                "class.fighter.feat",
                CharacterClassRuleKind.ClassFeatChoice,
                "Fighter Feat",
                "Choose a feat.",
                true,
                [ CharacterClassDependencyType.ClassFeatCatalog ] ) ] );
        FeatDefinition fighterFeat = Feat( "feat.double_slice", "Double Slice", FeatCategory.Class, "Fighter" );

        IReadOnlyList<FeatChoice> result = CharacterFeatResolver.ResolveClassChoices(
            fighter,
            null,
            null,
            [ new FeatChoice( "class.fighter.feat", fighterFeat.Id ) ],
            [ fighterFeat ] );

        Assert.Equal( fighterFeat.Id, Assert.Single( result ).FeatId );
        Assert.Throws<CharacterManagementException>( () => CharacterFeatResolver.ResolveClassChoices(
            fighter,
            null,
            null,
            [],
            [ fighterFeat ] ) );
    }

    [Fact]
    public void ResolveClassChoices_RejectsFeatAlreadyGrantedByClassChoice()
    {
        CharacterClass wizard = CreateClass( "wizard", "Wizard", [] );
        ArcaneSchool school = new ArcaneSchool(
            "arcane_school.unified_magical_theory",
            "Unified Magical Theory",
            SourceReference.Unknown,
            [],
            [
                new ArcaneSchoolBenefitDescriptor(
                    "spell.hand_of_the_apprentice",
                    ArcaneSchoolBenefitKind.InitialSchoolSpell,
                    "Hand of the Apprentice",
                    "Initial school spell.",
                    [] ),
                new ArcaneSchoolBenefitDescriptor(
                    "spell.interdisciplinary_incantation",
                    ArcaneSchoolBenefitKind.AdvancedSchoolSpell,
                    "Interdisciplinary Incantation",
                    "Advanced school spell.",
                    [ CharacterClassDependencyType.SpellCatalog ] ),
                new ArcaneSchoolBenefitDescriptor(
                    "arcane_school.unified_magical_theory.benefit.extra_class_feat",
                    ArcaneSchoolBenefitKind.ExtraClassFeat,
                    "Extra Wizard Class Feat",
                    "Choose an extra feat.",
                    [ CharacterClassDependencyType.ClassFeatCatalog ] ),
                new ArcaneSchoolBenefitDescriptor(
                    "arcane_school.unified_magical_theory.benefit.extra_spellbook_spell_choice",
                    ArcaneSchoolBenefitKind.ExtraSpellbookSpellChoice,
                    "Extra Spellbook Spell",
                    "Choose an extra spell.",
                    [] ),
                new ArcaneSchoolBenefitDescriptor(
                    "arcane_school.unified_magical_theory.benefit.additional_drain_bonded_item_uses",
                    ArcaneSchoolBenefitKind.AdditionalDrainBondedItemUses,
                    "Additional Drain Bonded Item Uses",
                    "Additional uses.",
                    [ CharacterClassDependencyType.ClassFeatureRules ] ),
            ] );
        FeatDefinition familiar = Feat( "feat.familiar", "Familiar", FeatCategory.Class, "Wizard" );

        Assert.Throws<CharacterManagementException>( () => CharacterFeatResolver.ResolveClassChoices(
            wizard,
            school,
            null,
            [ new FeatChoice( "arcane_school.unified_magical_theory.benefit.extra_class_feat", familiar.Id ) ],
            [ familiar ],
            [ familiar.Id ] ) );
    }

    private static CharacterClass CreateClass(
        string id,
        string name,
        IReadOnlyList<CharacterClassRuleDescriptor> rules )
    {
        return new CharacterClass(
            $"class.{id}",
            name,
            SourceReference.Unknown,
            10,
            [ AbilityType.Strength ],
            [ new ProficiencyGrant( ProficiencyTargets.Perception, ProficiencyRank.Trained, "test.perception" ) ],
            [],
            1,
            null,
            rules,
            [] );
    }

    private static Ancestry CreateAncestry()
    {
        return new Ancestry(
            AncestryType.Human,
            [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
            [],
            8,
            RaceSizeType.Medium,
            25,
            heritages:
            [
                new Heritage(
                    "human.skilled",
                    AncestryType.Human,
                    "Skilled Human",
                    SourceReference.Unknown,
                    AncestryChoiceRarity.Common,
                    [],
                    [],
                    [],
                    [] ),
            ],
            ancestryFeats:
            [
                new AncestryFeat(
                    "human.cooperative_nature",
                    AncestryType.Human,
                    "Cooperative Nature",
                    SourceReference.Unknown,
                    1,
                    AncestryChoiceRarity.Common,
                    [],
                    [],
                    [],
                    [],
                    [] ),
            ] );
    }

    private static Background CreateBackground()
    {
        return new Background(
            "background.acrobat",
            "Acrobat",
            SourceReference.Unknown,
            [ AbilityType.Strength, AbilityType.Dexterity ],
            1,
            [
                Grant( "background.acrobat.skill", BackgroundGrantKind.SkillTraining, "skill.acrobatics", "Acrobatics" ),
                Grant( "background.acrobat.lore", BackgroundGrantKind.LoreTraining, "lore.circus", "Circus Lore" ),
                Grant( "background.acrobat.feat", BackgroundGrantKind.SkillFeat, "skill_feat.steady_balance", "Steady Balance" ),
            ] );
    }

    private static BackgroundGrantDescriptor Grant(
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

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        return
        [
            new SkillDefinition(
                "skill.acrobatics",
                "Acrobatics",
                AbilityType.Dexterity,
                SourceReference.Unknown ),
        ];
    }

    private static FeatDefinition Feat(
        string id,
        string name,
        FeatCategory category,
        string trait )
    {
        return new FeatDefinition(
            id,
            name,
            category,
            1,
            [ trait ],
            FeatRarity.Common,
            [],
            "Deferred effect.",
            [ FeatDependencyType.RuleEngine ],
            SourceReference.Unknown );
    }
}
