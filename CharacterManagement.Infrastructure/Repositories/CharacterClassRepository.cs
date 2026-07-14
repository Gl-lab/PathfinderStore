using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class CharacterClassRepository : ICharacterClassRepository
{
    private static readonly Dictionary<string, CharacterClass> CharacterClasses = CreateCharacterClasses()
        .ToDictionary( characterClass => characterClass.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<CharacterClass> GetAll() => CharacterClasses.Values.ToList();

    public CharacterClass GetCharacterClass( string characterClassId )
    {
        if ( String.IsNullOrWhiteSpace( characterClassId ) )
        {
            throw new ArgumentException( "Character class id cannot be empty.", nameof( characterClassId ) );
        }

        if ( !CharacterClasses.TryGetValue( characterClassId, out CharacterClass? characterClass ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( characterClassId ),
                $"Character class '{characterClassId}' is not defined." );
        }

        return characterClass;
    }

    private static IReadOnlyCollection<CharacterClass> CreateCharacterClasses()
    {
        return
        [
            Create(
                "bard", "Bard", 94, 8, [ AbilityType.Charisma ], 4, SpellTradition.Occult,
                [
                    Spellcasting( "bard", "Occult spellcasting, spell repertoire, and composition spells." ),
                    Choice( "bard.muse", "Muse", "Choose a muse that grants a bard feat and a spell." )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.ClassFeatCatalog ] ),
            Create(
                "cleric", "Cleric", 108, 8, [ AbilityType.Wisdom ], 2, SpellTradition.Divine,
                [
                    Spellcasting( "cleric", "Divine spellcasting and Divine Font." ),
                    Choice( "cleric.deity", "Deity", "Choose a deity that determines a skill, favored weapon, and spells.", CharacterClassDependencyType.DeityCatalog ),
                    Choice( "cleric.doctrine", "Doctrine", "Choose a cleric doctrine.", CharacterClassDependencyType.ClericDoctrineCatalog )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.DeityCatalog, CharacterClassDependencyType.ClericDoctrineCatalog ] ),
            Create(
                "druid", "Druid", 122, 8, [ AbilityType.Wisdom ], 2, SpellTradition.Primal,
                [
                    Spellcasting( "druid", "Primal spellcasting." ),
                    Feature( "druid.features", "Druid Features", "Anathema, Shield Block, Voice of Nature, and Wildsong." ),
                    Choice( "druid.order", "Druidic Order", "Choose an order that grants a feat, order spell, and trained skill." )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.ClassFeatureRules ] ),
            Create(
                "fighter", "Fighter", 136, 10, [ AbilityType.Strength, AbilityType.Dexterity ], 3, null,
                [
                    Feature( "fighter.features", "Fighter Features", "Reactive Strike and Shield Block." ),
                    ClassFeat( "fighter.feat", "Fighter Feat" )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.ClassFeatCatalog ] ),
            Create(
                "ranger", "Ranger", 152, 10, [ AbilityType.Strength, AbilityType.Dexterity ], 4, null,
                [
                    Feature( "ranger.hunt_prey", "Hunt Prey", "Grants the Hunt Prey class feature." ),
                    Choice( "ranger.hunters_edge", "Hunter's Edge", "Choose a Hunter's Edge.", CharacterClassDependencyType.ClassChoiceCatalog ),
                    ClassFeat( "ranger.feat", "Ranger Feat" )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.ClassChoiceCatalog, CharacterClassDependencyType.ClassFeatCatalog ] ),
            Create(
                "rogue", "Rogue", 164, 8, [ AbilityType.Dexterity ], 7, null,
                [
                    Feature( "rogue.features", "Rogue Features", "Sneak Attack 1d6 and Surprise Attack." ),
                    Choice( "rogue.racket", "Rogue's Racket", "Choose a racket that can change skills and the available key ability.", CharacterClassDependencyType.RogueRacketCatalog ),
                    ClassFeat( "rogue.feat", "Rogue Feat" ),
                    SkillFeat( "rogue.skill_feat", "Skill Feat" )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.RogueRacketCatalog, CharacterClassDependencyType.ClassFeatCatalog, CharacterClassDependencyType.SkillFeatCatalog ] ),
            Create(
                "witch", "Witch", 178, 6, [ AbilityType.Intelligence ], 3, null,
                [
                    Spellcasting( "witch", "Spell tradition and spellcasting depend on the patron." ),
                    Feature( "witch.familiar", "Familiar", "Grants a familiar and familiar spell rules." ),
                    Choice( "witch.patron", "Patron", "Choose a patron that determines tradition and related rules." )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.FamiliarRules, CharacterClassDependencyType.ClassFeatureRules ] ),
            Create(
                "wizard", "Wizard", 192, 6, [ AbilityType.Intelligence ], 2, SpellTradition.Arcane,
                [
                    Spellcasting( "wizard", "Arcane spellcasting." ),
                    Feature( "wizard.arcane_bond", "Arcane Bond", "Grants the Arcane Bond class feature." ),
                    Choice( "wizard.thesis", "Arcane Thesis", "Choose an Arcane Thesis.", CharacterClassDependencyType.ClassChoiceCatalog ),
                    Choice( "wizard.school", "Arcane School", "Choose an Arcane School." )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.ClassChoiceCatalog ] )
        ];
    }

    private static CharacterClass Create(
        string id,
        string name,
        int page,
        int baseHitPoints,
        IReadOnlyList<AbilityType> keyAbilityOptions,
        int additionalSkillCountBase,
        SpellTradition? spellTradition,
        IReadOnlyList<CharacterClassRuleDescriptor> rules,
        IReadOnlyList<CharacterClassDependencyType> dependencies )
    {
        return new CharacterClass(
            $"class.{id}",
            name,
            new SourceReference( "Player Core", page ),
            baseHitPoints,
            keyAbilityOptions,
            InitialProficiencies( id, name ),
            InitialSkillGrants( id ),
            additionalSkillCountBase,
            spellTradition,
            [ AdditionalSkills( id, additionalSkillCountBase ), .. rules ],
            dependencies );
    }

    private static IReadOnlyList<ClassSkillGrantDescriptor> InitialSkillGrants( string id )
    {
        return id switch
        {
            "bard" =>
            [
                SkillGrant( "bard", "occultism", "skill.occultism" ),
                SkillGrant( "bard", "performance", "skill.performance" ),
            ],
            "cleric" => [ SkillGrant( "cleric", "religion", "skill.religion" ) ],
            "druid" => [ SkillGrant( "druid", "nature", "skill.nature" ) ],
            "fighter" =>
            [
                SkillGrant(
                    "fighter",
                    "acrobatics_or_athletics",
                    "skill.acrobatics",
                    "skill.athletics" ),
            ],
            "ranger" =>
            [
                SkillGrant( "ranger", "nature", "skill.nature" ),
                SkillGrant( "ranger", "survival", "skill.survival" ),
            ],
            "rogue" => [ SkillGrant( "rogue", "stealth", "skill.stealth" ) ],
            "witch" => [],
            "wizard" => [ SkillGrant( "wizard", "arcana", "skill.arcana" ) ],
            _ => throw new ArgumentOutOfRangeException( nameof( id ), $"Unknown character class '{id}'." ),
        };
    }

    private static ClassSkillGrantDescriptor SkillGrant(
        string classId,
        string grantId,
        params string[] skillOptions )
    {
        return new ClassSkillGrantDescriptor(
            $"class.{classId}.skill.{grantId}",
            skillOptions );
    }

    private static IReadOnlyList<ProficiencyGrant> InitialProficiencies( string id, string name )
    {
        IReadOnlyList<(ProficiencyTarget Target, ProficiencyRank Rank)> definitions = id switch
        {
            "bard" =>
            [
                ( ProficiencyTargets.Perception, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Fortitude, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Reflex, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Will, ProficiencyRank.Expert ),
                ( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.MartialWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmedAttacks, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmoredDefense, ProficiencyRank.Trained ),
                ( ProficiencyTargets.LightArmor, ProficiencyRank.Trained ),
            ],
            "cleric" =>
            [
                ( ProficiencyTargets.Perception, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Fortitude, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Reflex, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Will, ProficiencyRank.Expert ),
                ( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmedAttacks, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmoredDefense, ProficiencyRank.Trained ),
            ],
            "druid" =>
            [
                ( ProficiencyTargets.Perception, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Fortitude, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Reflex, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Will, ProficiencyRank.Expert ),
                ( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmedAttacks, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmoredDefense, ProficiencyRank.Trained ),
                ( ProficiencyTargets.LightArmor, ProficiencyRank.Trained ),
                ( ProficiencyTargets.MediumArmor, ProficiencyRank.Trained ),
            ],
            "fighter" =>
            [
                ( ProficiencyTargets.Perception, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Fortitude, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Reflex, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Will, ProficiencyRank.Trained ),
                ( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Expert ),
                ( ProficiencyTargets.MartialWeapons, ProficiencyRank.Expert ),
                ( ProficiencyTargets.AdvancedWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmedAttacks, ProficiencyRank.Expert ),
                ( ProficiencyTargets.UnarmoredDefense, ProficiencyRank.Trained ),
                ( ProficiencyTargets.LightArmor, ProficiencyRank.Trained ),
                ( ProficiencyTargets.MediumArmor, ProficiencyRank.Trained ),
                ( ProficiencyTargets.HeavyArmor, ProficiencyRank.Trained ),
            ],
            "ranger" =>
            [
                ( ProficiencyTargets.Perception, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Fortitude, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Reflex, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Will, ProficiencyRank.Trained ),
                ( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.MartialWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmedAttacks, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmoredDefense, ProficiencyRank.Trained ),
                ( ProficiencyTargets.LightArmor, ProficiencyRank.Trained ),
                ( ProficiencyTargets.MediumArmor, ProficiencyRank.Trained ),
            ],
            "rogue" =>
            [
                ( ProficiencyTargets.Perception, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Fortitude, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Reflex, ProficiencyRank.Expert ),
                ( ProficiencyTargets.Will, ProficiencyRank.Expert ),
                ( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.MartialWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmedAttacks, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmoredDefense, ProficiencyRank.Trained ),
                ( ProficiencyTargets.LightArmor, ProficiencyRank.Trained ),
            ],
            "witch" or "wizard" =>
            [
                ( ProficiencyTargets.Perception, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Fortitude, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Reflex, ProficiencyRank.Trained ),
                ( ProficiencyTargets.Will, ProficiencyRank.Expert ),
                ( ProficiencyTargets.SimpleWeapons, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmedAttacks, ProficiencyRank.Trained ),
                ( ProficiencyTargets.UnarmoredDefense, ProficiencyRank.Trained ),
            ],
            _ => throw new ArgumentOutOfRangeException( nameof( id ), $"Unknown character class '{id}'." ),
        };

        string sourceGrantId = $"class.{id}.initial_proficiencies";
        List<ProficiencyGrant> grants = definitions
            .Select( definition => new ProficiencyGrant(
                definition.Target,
                definition.Rank,
                sourceGrantId ) )
            .ToList();

        grants.Add( new ProficiencyGrant(
            ProficiencyTargets.ClassDc( $"class.{id}", name ),
            ProficiencyRank.Trained,
            sourceGrantId ) );

        return grants;
    }

    private static CharacterClassRuleDescriptor AdditionalSkills( string id, int baseCount )
    {
        return Rule(
            $"class.{id}.additional_skills",
            CharacterClassRuleKind.AdditionalSkills,
            "Additional Skills",
            $"Trained in {baseCount} plus Intelligence modifier additional skills.",
            true,
            [] );
    }

    private static CharacterClassRuleDescriptor Spellcasting( string id, string summary )
    {
        return Rule(
            $"class.{id}.spellcasting",
            CharacterClassRuleKind.Spellcasting,
            "Spellcasting",
            summary,
            true,
            CharacterClassDependencyType.SpellCatalog );
    }

    private static CharacterClassRuleDescriptor Feature( string id, string name, string summary )
    {
        return Rule(
            $"class.{id}",
            CharacterClassRuleKind.GrantedFeature,
            name,
            summary,
            false,
            CharacterClassDependencyType.ClassFeatureRules );
    }

    private static CharacterClassRuleDescriptor Choice(
        string id,
        string name,
        string summary,
        params CharacterClassDependencyType[] dependencies )
    {
        return Rule(
            $"class_choice.{id}",
            CharacterClassRuleKind.MandatoryChoice,
            name,
            summary,
            true,
            dependencies );
    }

    private static CharacterClassRuleDescriptor ClassFeat( string id, string name )
    {
        return Rule(
            $"class_choice.{id}",
            CharacterClassRuleKind.ClassFeatChoice,
            name,
            $"Choose a 1st-level {name.ToLowerInvariant()}.",
            true,
            CharacterClassDependencyType.ClassFeatCatalog );
    }

    private static CharacterClassRuleDescriptor SkillFeat( string id, string name )
    {
        return Rule(
            $"class_choice.{id}",
            CharacterClassRuleKind.SkillFeatChoice,
            name,
            "Choose a 1st-level skill feat.",
            true,
            CharacterClassDependencyType.SkillFeatCatalog );
    }

    private static CharacterClassRuleDescriptor Rule(
        string id,
        CharacterClassRuleKind kind,
        string name,
        string summary,
        bool requiresChoice,
        params CharacterClassDependencyType[] dependencies )
    {
        return new CharacterClassRuleDescriptor(
            id,
            kind,
            name,
            summary,
            requiresChoice,
            dependencies );
    }
}
