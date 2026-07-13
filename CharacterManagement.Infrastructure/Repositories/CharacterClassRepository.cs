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
                "bard", "Bard", 94, 8, [ AbilityType.Charisma ], SpellTradition.Occult,
                [
                    Proficiencies( "bard", "Expert Perception; trained Fortitude and Reflex; expert Will; trained Occultism, Performance, weapons, defenses, bard class DC, and spell attack/DC." ),
                    AdditionalSkills( "bard", 4 ),
                    Spellcasting( "bard", "Occult spellcasting, spell repertoire, and composition spells." ),
                    Choice( "bard.muse", "Muse", "Choose a muse that grants a bard feat and a spell.", CharacterClassDependencyType.ClassChoiceCatalog )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.ClassChoiceCatalog, CharacterClassDependencyType.ClassFeatCatalog ] ),
            Create(
                "cleric", "Cleric", 108, 8, [ AbilityType.Wisdom ], SpellTradition.Divine,
                [
                    Proficiencies( "cleric", "Trained Perception, Fortitude, and Reflex; expert Will; trained Religion, weapons, defenses, cleric class DC, and spell attack/DC; deity and doctrine modify training." ),
                    AdditionalSkills( "cleric", 2 ),
                    Spellcasting( "cleric", "Divine spellcasting and Divine Font." ),
                    Choice( "cleric.deity", "Deity", "Choose a deity that determines a skill, favored weapon, and spells.", CharacterClassDependencyType.DeityCatalog ),
                    Choice( "cleric.doctrine", "Doctrine", "Choose a cleric doctrine.", CharacterClassDependencyType.ClassChoiceCatalog )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.DeityCatalog, CharacterClassDependencyType.ClassChoiceCatalog ] ),
            Create(
                "druid", "Druid", 122, 8, [ AbilityType.Wisdom ], SpellTradition.Primal,
                [
                    Proficiencies( "druid", "Trained Perception, Fortitude, and Reflex; expert Will; trained Nature, weapons, and defenses; order determines an additional skill." ),
                    AdditionalSkills( "druid", 2 ),
                    Spellcasting( "druid", "Primal spellcasting." ),
                    Feature( "druid.features", "Druid Features", "Anathema, Shield Block, Voice of Nature, and Wildsong." ),
                    Choice( "druid.order", "Druidic Order", "Choose an order that grants a feat, order spell, and trained skill.", CharacterClassDependencyType.ClassChoiceCatalog )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.ClassChoiceCatalog ] ),
            Create(
                "fighter", "Fighter", 136, 10, [ AbilityType.Strength, AbilityType.Dexterity ], null,
                [
                    Proficiencies( "fighter", "Expert Perception, Fortitude, and Reflex; trained Will; expert simple and martial weapons; trained advanced weapons, armor, and defenses." ),
                    AdditionalSkills( "fighter", 3 ),
                    Feature( "fighter.features", "Fighter Features", "Reactive Strike and Shield Block." ),
                    ClassFeat( "fighter.feat", "Fighter Feat" )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.ClassFeatCatalog ] ),
            Create(
                "ranger", "Ranger", 152, 10, [ AbilityType.Strength, AbilityType.Dexterity ], null,
                [
                    Proficiencies( "ranger", "Expert Perception, Fortitude, and Reflex; trained Will; trained Nature, Survival, weapons, defenses, and ranger class DC." ),
                    AdditionalSkills( "ranger", 4 ),
                    Feature( "ranger.hunt_prey", "Hunt Prey", "Grants the Hunt Prey class feature." ),
                    Choice( "ranger.hunters_edge", "Hunter's Edge", "Choose a Hunter's Edge.", CharacterClassDependencyType.ClassChoiceCatalog ),
                    ClassFeat( "ranger.feat", "Ranger Feat" )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.ClassChoiceCatalog, CharacterClassDependencyType.ClassFeatCatalog ] ),
            Create(
                "rogue", "Rogue", 164, 8, [ AbilityType.Dexterity ], null,
                [
                    Proficiencies( "rogue", "Expert Perception, Reflex, and Will; trained Fortitude, Stealth, weapons, defenses, and rogue class DC; racket modifies skills." ),
                    AdditionalSkills( "rogue", 7 ),
                    Feature( "rogue.features", "Rogue Features", "Sneak Attack 1d6 and Surprise Attack." ),
                    Choice( "rogue.racket", "Rogue's Racket", "Choose a racket that can change skills and the available key ability.", CharacterClassDependencyType.RogueRacketCatalog ),
                    ClassFeat( "rogue.feat", "Rogue Feat" ),
                    SkillFeat( "rogue.skill_feat", "Skill Feat" )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.RogueRacketCatalog, CharacterClassDependencyType.ClassFeatCatalog, CharacterClassDependencyType.SkillFeatCatalog ] ),
            Create(
                "witch", "Witch", 178, 6, [ AbilityType.Intelligence ], null,
                [
                    Proficiencies( "witch", "Trained Perception, Fortitude, and Reflex; expert Will; trained weapons, defenses, witch class DC, and spell attack/DC; patron determines a skill." ),
                    AdditionalSkills( "witch", 3 ),
                    Spellcasting( "witch", "Spell tradition and spellcasting depend on the patron." ),
                    Feature( "witch.familiar", "Familiar", "Grants a familiar and familiar spell rules." ),
                    Choice( "witch.patron", "Patron", "Choose a patron that determines tradition and related rules.", CharacterClassDependencyType.ClassChoiceCatalog )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.FamiliarRules, CharacterClassDependencyType.ClassChoiceCatalog ] ),
            Create(
                "wizard", "Wizard", 192, 6, [ AbilityType.Intelligence ], SpellTradition.Arcane,
                [
                    Proficiencies( "wizard", "Trained Perception, Fortitude, and Reflex; expert Will; trained Arcana, weapons, defenses, wizard class DC, and spell attack/DC." ),
                    AdditionalSkills( "wizard", 2 ),
                    Spellcasting( "wizard", "Arcane spellcasting." ),
                    Feature( "wizard.arcane_bond", "Arcane Bond", "Grants the Arcane Bond class feature." ),
                    Choice( "wizard.thesis", "Arcane Thesis", "Choose an Arcane Thesis.", CharacterClassDependencyType.ClassChoiceCatalog ),
                    Choice( "wizard.school", "Arcane School", "Choose an Arcane School.", CharacterClassDependencyType.ClassChoiceCatalog )
                ],
                [ CharacterClassDependencyType.ProficiencyRules, CharacterClassDependencyType.SkillCatalog, CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.ClassFeatureRules, CharacterClassDependencyType.ClassChoiceCatalog ] )
        ];
    }

    private static CharacterClass Create(
        string id,
        string name,
        int page,
        int baseHitPoints,
        IReadOnlyList<AbilityType> keyAbilityOptions,
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
            spellTradition,
            rules,
            dependencies );
    }

    private static CharacterClassRuleDescriptor Proficiencies( string id, string summary )
    {
        return Rule(
            $"class.{id}.initial_proficiencies",
            CharacterClassRuleKind.InitialProficiencies,
            "Initial Proficiencies",
            summary,
            false,
            CharacterClassDependencyType.ProficiencyRules );
    }

    private static CharacterClassRuleDescriptor AdditionalSkills( string id, int baseCount )
    {
        return Rule(
            $"class.{id}.additional_skills",
            CharacterClassRuleKind.AdditionalSkills,
            "Additional Skills",
            $"Trained in {baseCount} plus Intelligence modifier additional skills.",
            true,
            CharacterClassDependencyType.SkillCatalog,
            CharacterClassDependencyType.ProficiencyRules );
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
        CharacterClassDependencyType dependency )
    {
        return Rule(
            $"class_choice.{id}",
            CharacterClassRuleKind.MandatoryChoice,
            name,
            summary,
            true,
            dependency );
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
