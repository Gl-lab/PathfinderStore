using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class AncestryRepository : IAncestryRepository
{
    private static readonly Dictionary<AncestryType, Ancestry> Ancestries = new()
    {
        [ AncestryType.Dwarf ] = CreateDwarf(),
        [ AncestryType.Elf ] = CreateElf(),
        [ AncestryType.Gnome ] = CreateGnome(),
        [ AncestryType.Goblin ] = CreateGoblin(),
        [ AncestryType.Halfling ] = CreateHalfling(),
        [ AncestryType.Human ] = CreateHuman(),
    };

    public IReadOnlyCollection<Ancestry> GetAll() => Ancestries.Values.ToList();

    public Ancestry GetAncestry( AncestryType ancestryType )
    {
        if ( !Ancestries.TryGetValue( ancestryType, out Ancestry? ancestry ) )
        {
            throw new ArgumentOutOfRangeException( nameof( ancestryType ), $"Ancestry '{ancestryType}' is not defined." );
        }

        return ancestry;
    }

    private static Ancestry CreateDwarf()
    {
        return new Ancestry(
            AncestryType.Dwarf,
            [
                AncestryBoostSlot.Fixed( AbilityType.Constitution ),
                AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
                AncestryBoostSlot.Free()
            ],
            [ AbilityType.Charisma ],
            10,
            RaceSizeType.Medium,
            20,
            source: Source( 43 ),
            vision: VisionType.Darkvision,
            startingLanguages: [ LanguageIds.Common, LanguageIds.Dwarven ],
            additionalLanguageRule: AdditionalLanguages(
                AdditionalLanguageRuleType.IntelligenceModifier,
                [ LanguageIds.Gnomish, LanguageIds.Goblin, LanguageIds.Jotun, LanguageIds.Orcish, LanguageIds.Petran, LanguageIds.Sakvroth ] ),
            grantedItems: [ new GrantedItem( "clan_dagger", 1, Source( 277 ) ) ],
            grantedRules: [ new GrantedRule( "dwarf.clan_dagger_taboo", AncestryEffectKind.RuleEffect, "Clan dagger sale is a cultural taboo." ) ],
            heritages:
            [
                Heritage( "dwarf.ancient_blooded", "Ancient-Blooded Dwarf", 43, AncestryEffectKind.RuleEffect, "Call on Ancient Blood reaction.", [ AncestryDependencyType.RuleEngine ] ),
                Heritage( "dwarf.death_warden", "Death Warden Dwarf", 43, AncestryEffectKind.RuleEffect, "Improves saves against void and undead effects.", [ AncestryDependencyType.RuleEngine, AncestryDependencyType.CombatRules ] ),
                Heritage( "dwarf.forge", "Forge Dwarf", 43, AncestryEffectKind.RuleEffect, "Fire resistance and environmental heat protection.", [ AncestryDependencyType.ResistanceRules, AncestryDependencyType.EnvironmentRules ] ),
                Heritage( "dwarf.rock", "Rock Dwarf", 43, AncestryEffectKind.RuleEffect, "Protection from forced movement.", [ AncestryDependencyType.CombatRules, AncestryDependencyType.MovementRules ] ),
                Heritage( "dwarf.strong_blooded", "Strong-Blooded Dwarf", 43, AncestryEffectKind.RuleEffect, "Poison resistance and affliction improvement.", [ AncestryDependencyType.ResistanceRules, AncestryDependencyType.ConditionRules ] )
            ],
            ancestryFeats:
            [
                Feat( "dwarf.dwarven_doughtiness", "Dwarven Doughtiness", 43, "Reduces frightened at end of turn.", [ AncestryDependencyType.ConditionRules ] ),
                Feat( "dwarf.dwarven_lore", "Dwarven Lore", 43, "Grants Crafting, Religion, and Dwarf Lore.", [ AncestryDependencyType.ProficiencyRules ] ),
                Feat( "dwarf.dwarven_weapon_familiarity", "Dwarven Weapon Familiarity", 44, "Grants dwarf weapon familiarity.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.ProficiencyRules ] ),
                Feat( "dwarf.mountain_strategy", "Mountain Strategy", 44, "Adds damage against specified creature traits.", [ AncestryDependencyType.CombatRules ] ),
                Feat( "dwarf.rock_runner", "Rock Runner", 44, "Improves movement over rocky terrain.", [ AncestryDependencyType.MovementRules ] ),
                Feat( "dwarf.stonemasons_eye", "Stonemason's Eye", 44, "Grants masonry perception benefits.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.PerceptionRules ] ),
                Feat( "dwarf.unburdened_iron", "Unburdened Iron", 44, "Reduces armor speed penalties.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.MovementRules ] )
            ] );
    }

    private static Ancestry CreateElf()
    {
        return new Ancestry(
            AncestryType.Elf,
            [
                AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
                AncestryBoostSlot.Fixed( AbilityType.Intelligence ),
                AncestryBoostSlot.Free()
            ],
            [ AbilityType.Constitution ],
            6,
            RaceSizeType.Medium,
            30,
            source: Source( 47 ),
            vision: VisionType.LowLight,
            startingLanguages: [ LanguageIds.Common, LanguageIds.Elven ],
            additionalLanguageRule: AdditionalLanguages(
                AdditionalLanguageRuleType.IntelligenceModifier,
                [ LanguageIds.Draconic, LanguageIds.Empyrean, LanguageIds.Fey, LanguageIds.Gnomish, LanguageIds.Goblin, LanguageIds.Kholo, LanguageIds.Orcish ] ),
            heritages:
            [
                Heritage( "elf.ancient", "Ancient Elf", 47, AncestryEffectKind.DeferredChoice, "Choose a multiclass dedication.", [ AncestryDependencyType.ClassCatalog, AncestryDependencyType.ClassFeatCatalog, AncestryDependencyType.ArchetypeCatalog ] ),
                Heritage( "elf.arctic", "Arctic Elf", 47, AncestryEffectKind.RuleEffect, "Cold resistance and environmental cold protection.", [ AncestryDependencyType.ResistanceRules, AncestryDependencyType.EnvironmentRules ] ),
                Heritage( "elf.cavern", "Cavern Elf", 47, AncestryEffectKind.VisionOverride, "Grants darkvision.", [], visionOverride: VisionType.Darkvision ),
                Heritage( "elf.seer", "Seer Elf", 47, AncestryEffectKind.DeferredChoice, "Grants detect magic and related bonuses.", [ AncestryDependencyType.SpellCatalog, AncestryDependencyType.Spellcasting, AncestryDependencyType.SkillCatalog ] ),
                Heritage( "elf.whisper", "Whisper Elf", 47, AncestryEffectKind.RuleEffect, "Improves Seek and hearing interactions.", [ AncestryDependencyType.PerceptionRules, AncestryDependencyType.ConditionRules ] ),
                Heritage( "elf.woodland", "Woodland Elf", 47, AncestryEffectKind.RuleEffect, "Improves forest movement and cover.", [ AncestryDependencyType.MovementRules, AncestryDependencyType.EnvironmentRules ] )
            ],
            ancestryFeats:
            [
                Feat( "elf.ancestral_longevity", "Ancestral Longevity", 47, "Temporarily gains a trained skill.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.RuleEngine ], AncestryEffectKind.DeferredChoice ),
                Feat( "elf.elven_lore", "Elven Lore", 47, "Grants skills and Elven Lore.", [ AncestryDependencyType.ProficiencyRules ] ),
                Feat( "elf.elven_weapon_familiarity", "Elven Weapon Familiarity", 48, "Grants elven weapon familiarity.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.ProficiencyRules ] ),
                Feat( "elf.forlorn", "Forlorn", 48, "Changes emotion and mental effect outcomes.", [ AncestryDependencyType.ConditionRules ] ),
                Feat( "elf.nimble_elf", "Nimble Elf", 48, "Increases Speed.", [ AncestryDependencyType.MovementRules ] ),
                Feat( "elf.otherworldly_magic", "Otherworldly Magic", 48, "Choose an innate cantrip.", [ AncestryDependencyType.SpellCatalog ], AncestryEffectKind.DeferredChoice ),
                Feat( "elf.unwavering_mien", "Unwavering Mien", 48, "Improves saves against emotion effects.", [ AncestryDependencyType.ConditionRules, AncestryDependencyType.CombatRules ] )
            ] );
    }

    private static Ancestry CreateGnome()
    {
        return new Ancestry(
            AncestryType.Gnome,
            [
                AncestryBoostSlot.Fixed( AbilityType.Constitution ),
                AncestryBoostSlot.Fixed( AbilityType.Charisma ),
                AncestryBoostSlot.Free()
            ],
            [ AbilityType.Strength ],
            8,
            RaceSizeType.Small,
            25,
            source: Source( 51 ),
            vision: VisionType.LowLight,
            startingLanguages: [ LanguageIds.Common, LanguageIds.Fey, LanguageIds.Gnomish ],
            additionalLanguageRule: AdditionalLanguages(
                AdditionalLanguageRuleType.IntelligenceModifier,
                [ LanguageIds.Draconic, LanguageIds.Dwarven, LanguageIds.Elven, LanguageIds.Goblin, LanguageIds.Jotun, LanguageIds.Orcish ] ),
            heritages:
            [
                Heritage( "gnome.chameleon", "Chameleon Gnome", 51, AncestryEffectKind.RuleEffect, "Changes coloration and grants Stealth benefits.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.ConditionRules ] ),
                Heritage( "gnome.fey_touched", "Fey-Touched Gnome", 51, AncestryEffectKind.DeferredChoice, "Choose a primal innate cantrip.", [ AncestryDependencyType.SpellCatalog, AncestryDependencyType.Spellcasting ] ),
                Heritage( "gnome.sensate", "Sensate Gnome", 51, AncestryEffectKind.RuleEffect, "Grants imprecise scent and perception benefits.", [ AncestryDependencyType.PerceptionRules ] ),
                Heritage( "gnome.umbral", "Umbral Gnome", 51, AncestryEffectKind.VisionOverride, "Grants darkvision.", [], visionOverride: VisionType.Darkvision ),
                Heritage( "gnome.wellspring", "Wellspring Gnome", 51, AncestryEffectKind.DeferredChoice, "Choose spell tradition and innate cantrip.", [ AncestryDependencyType.SpellCatalog, AncestryDependencyType.Spellcasting ] )
            ],
            ancestryFeats:
            [
                Feat( "gnome.animal_accomplice", "Animal Accomplice", 51, "Grants an animal companion.", [ AncestryDependencyType.AnimalCompanionRules ] ),
                Feat( "gnome.animal_elocutionist", "Animal Elocutionist", 52, "Communicates with animals.", [ AncestryDependencyType.Spellcasting ] ),
                Feat( "gnome.fey_fellowship", "Fey Fellowship", 52, "Changes diplomacy with fey.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.CombatRules ] ),
                Feat( "gnome.first_world_magic", "First World Magic", 52, "Choose an innate cantrip.", [ AncestryDependencyType.SpellCatalog ], AncestryEffectKind.DeferredChoice ),
                Feat( "gnome.gnome_obsession", "Gnome Obsession", 52, "Choose a Lore skill.", [ AncestryDependencyType.LoreCatalog ], AncestryEffectKind.DeferredChoice ),
                Feat( "gnome.gnome_weapon_familiarity", "Gnome Weapon Familiarity", 52, "Grants gnome weapon familiarity.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.ProficiencyRules ] ),
                Feat( "gnome.razzle_dazzle", "Razzle-Dazzle", 52, "Changes Performance outcomes.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.ConditionRules ] )
            ] );
    }

    private static Ancestry CreateGoblin()
    {
        return new Ancestry(
            AncestryType.Goblin,
            [
                AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
                AncestryBoostSlot.Fixed( AbilityType.Charisma ),
                AncestryBoostSlot.Free()
            ],
            [ AbilityType.Wisdom ],
            6,
            RaceSizeType.Small,
            25,
            source: Source( 55 ),
            vision: VisionType.Darkvision,
            startingLanguages: [ LanguageIds.Common, LanguageIds.Goblin ],
            additionalLanguageRule: AdditionalLanguages(
                AdditionalLanguageRuleType.IntelligenceModifier,
                [ LanguageIds.Draconic, LanguageIds.Dwarven, LanguageIds.Gnomish, LanguageIds.Halfling, LanguageIds.Kholo, LanguageIds.Orcish ] ),
            heritages:
            [
                Heritage( "goblin.charhide", "Charhide Goblin", 55, AncestryEffectKind.RuleEffect, "Fire resistance and persistent fire protection.", [ AncestryDependencyType.ResistanceRules, AncestryDependencyType.ConditionRules ] ),
                Heritage( "goblin.irongut", "Irongut Goblin", 55, AncestryEffectKind.RuleEffect, "Changes food, affliction, and sickened rules.", [ AncestryDependencyType.ConditionRules, AncestryDependencyType.RuleEngine ] ),
                Heritage( "goblin.razortooth", "Razortooth Goblin", 55, AncestryEffectKind.RuleEffect, "Grants jaws unarmed attack.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.CombatRules ] ),
                Heritage( "goblin.snow", "Snow Goblin", 55, AncestryEffectKind.RuleEffect, "Cold resistance and environmental cold protection.", [ AncestryDependencyType.ResistanceRules, AncestryDependencyType.EnvironmentRules ] ),
                Heritage( "goblin.unbreakable", "Unbreakable Goblin", 55, AncestryEffectKind.BaseHpOverride, "Sets ancestry HP to 10 and changes falling damage.", [ AncestryDependencyType.CombatRules ], baseHitPointsOverride: 10 )
            ],
            ancestryFeats:
            [
                Feat( "goblin.burn_it", "Burn It!", 55, "Increases fire damage.", [ AncestryDependencyType.Spellcasting, AncestryDependencyType.CombatRules ] ),
                Feat( "goblin.goblin_lore", "Goblin Lore", 55, "Grants skills and Goblin Lore.", [ AncestryDependencyType.ProficiencyRules ] ),
                Feat( "goblin.goblin_scuttle", "Goblin Scuttle", 56, "Reaction that grants a Step.", [ AncestryDependencyType.MovementRules, AncestryDependencyType.RuleEngine ] ),
                Feat( "goblin.goblin_song", "Goblin Song", 56, "Performance action that penalizes targets.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.ConditionRules ] ),
                Feat( "goblin.goblin_weapon_familiarity", "Goblin Weapon Familiarity", 56, "Grants goblin weapon familiarity.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.ProficiencyRules ] ),
                Feat( "goblin.rough_rider", "Rough Rider", 56, "Grants Ride and mount benefits.", [ AncestryDependencyType.GeneralFeatCatalog, AncestryDependencyType.AnimalCompanionRules ] ),
                Feat( "goblin.very_sneaky", "Very Sneaky", 56, "Improves Sneak movement.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.ConditionRules ] )
            ] );
    }

    private static Ancestry CreateHalfling()
    {
        return new Ancestry(
            AncestryType.Halfling,
            [
                AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
                AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
                AncestryBoostSlot.Free()
            ],
            [ AbilityType.Strength ],
            6,
            RaceSizeType.Small,
            25,
            source: Source( 59 ),
            vision: VisionType.None,
            startingLanguages: [ LanguageIds.Common, LanguageIds.Halfling ],
            additionalLanguageRule: AdditionalLanguages(
                AdditionalLanguageRuleType.IntelligenceModifier,
                [ LanguageIds.Dwarven, LanguageIds.Elven, LanguageIds.Gnomish, LanguageIds.Goblin ] ),
            grantedRules: [ new GrantedRule( "halfling.keen_eyes", AncestryEffectKind.RuleEffect, "Keen Eyes at 1st level." ) ],
            heritages:
            [
                Heritage( "halfling.gutsy", "Gutsy Halfling", 59, AncestryEffectKind.RuleEffect, "Improves saves against emotion effects.", [ AncestryDependencyType.ConditionRules, AncestryDependencyType.CombatRules ] ),
                Heritage( "halfling.hillock", "Hillock Halfling", 59, AncestryEffectKind.RuleEffect, "Improves healing during rest and Treat Wounds.", [ AncestryDependencyType.RuleEngine ] ),
                Heritage( "halfling.jinxed", "Jinxed Halfling", 59, AncestryEffectKind.DeferredChoice, "Grants Jinx action and restricts Halfling Luck.", [ AncestryDependencyType.ClassCatalog, AncestryDependencyType.Spellcasting, AncestryDependencyType.RuleEngine ], AncestryChoiceRarity.Uncommon, [ "Cannot select halfling.halfling_luck." ], [ "halfling.halfling_luck" ] ),
                Heritage( "halfling.nomadic", "Nomadic Halfling", 59, AncestryEffectKind.DeferredChoice, "Choose two additional languages.", [ AncestryDependencyType.LanguageCatalog ] ),
                Heritage( "halfling.twilight", "Twilight Halfling", 59, AncestryEffectKind.VisionOverride, "Grants low-light vision.", [], visionOverride: VisionType.LowLight ),
                Heritage( "halfling.wildwood", "Wildwood Halfling", 59, AncestryEffectKind.RuleEffect, "Ignores plant difficult terrain.", [ AncestryDependencyType.MovementRules, AncestryDependencyType.EnvironmentRules ] )
            ],
            ancestryFeats:
            [
                Feat( "halfling.distracting_shadows", "Distracting Shadows", 60, "Uses larger creatures as cover.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.ConditionRules ] ),
                Feat( "halfling.halfling_lore", "Halfling Lore", 60, "Grants skills and Halfling Lore.", [ AncestryDependencyType.ProficiencyRules ] ),
                Feat( "halfling.halfling_luck", "Halfling Luck", 60, "Rerolls a failed skill check or save.", [ AncestryDependencyType.RuleEngine ], incompatibleChoiceIds: [ "halfling.jinxed" ] ),
                Feat( "halfling.halfling_weapon_familiarity", "Halfling Weapon Familiarity", 60, "Grants halfling weapon familiarity.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.ProficiencyRules ] ),
                Feat( "halfling.prairie_rider", "Prairie Rider", 60, "Grants Nature and mount benefits.", [ AncestryDependencyType.AnimalCompanionRules ] ),
                Feat( "halfling.sure_feet", "Sure Feet", 60, "Improves Balance and Climb outcomes.", [ AncestryDependencyType.MovementRules, AncestryDependencyType.ConditionRules ] )
            ] );
    }

    private static Ancestry CreateHuman()
    {
        return new Ancestry(
            AncestryType.Human,
            [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
            [],
            8,
            RaceSizeType.Medium,
            25,
            source: Source( 63 ),
            vision: VisionType.None,
            startingLanguages: [ LanguageIds.Common ],
            additionalLanguageRule: new AdditionalLanguageRule(
                AdditionalLanguageRuleType.OnePlusIntelligenceModifier,
                [],
                true,
                true ),
            heritages:
            [
                Heritage( "human.skilled", "Skilled Human", 63, AncestryEffectKind.DeferredChoice, "Choose a trained skill with later proficiency progression.", [ AncestryDependencyType.SkillCatalog, AncestryDependencyType.ProficiencyRules ] ),
                Heritage( "human.versatile", "Versatile Human", 63, AncestryEffectKind.DeferredChoice, "Choose a qualifying general feat.", [ AncestryDependencyType.GeneralFeatCatalog ] )
            ],
            ancestryFeats:
            [
                Feat( "human.adapted_cantrip", "Adapted Cantrip", 63, "Choose a cantrip from another tradition.", [ AncestryDependencyType.ClassCatalog, AncestryDependencyType.SpellCatalog ], AncestryEffectKind.DeferredChoice, [ "Spellcasting class feature." ] ),
                Feat( "human.cooperative_nature", "Cooperative Nature", 64, "Improves Aid checks.", [ AncestryDependencyType.RuleEngine ] ),
                Feat( "human.general_training", "General Training", 64, "Choose a qualifying 1st-level general feat.", [ AncestryDependencyType.GeneralFeatCatalog ], AncestryEffectKind.DeferredChoice, [ "General feat prerequisites." ] ),
                Feat( "human.natural_ambition", "Natural Ambition", 64, "Choose a 1st-level class feat.", [ AncestryDependencyType.ClassCatalog, AncestryDependencyType.ClassFeatCatalog ], AncestryEffectKind.DeferredChoice ),
                Feat( "human.natural_skill", "Natural Skill", 64, "Choose two trained skills.", [ AncestryDependencyType.SkillCatalog ], AncestryEffectKind.DeferredChoice ),
                Feat( "human.unconventional_weaponry", "Unconventional Weaponry", 64, "Choose a qualifying uncommon weapon.", [ AncestryDependencyType.WeaponCatalog, AncestryDependencyType.ProficiencyRules ], AncestryEffectKind.DeferredChoice )
            ] );
    }

    private static SourceReference Source( int page ) => new( "Player Core", page );

    private static AdditionalLanguageRule AdditionalLanguages(
        AdditionalLanguageRuleType type,
        IReadOnlyList<LanguageId> languageIds ) => new AdditionalLanguageRule(
            type,
            languageIds,
            false,
            true );

    private static Heritage Heritage(
        string id,
        string name,
        int sourcePage,
        AncestryEffectKind effectKind,
        string summary,
        IReadOnlyList<AncestryDependencyType> dependencies,
        AncestryChoiceRarity rarity = AncestryChoiceRarity.Common,
        IReadOnlyList<string>? restrictions = null,
        IReadOnlyList<string>? incompatibleChoiceIds = null,
        VisionType? visionOverride = null,
        int? baseHitPointsOverride = null )
    {
        return new Heritage(
            id,
            AncestryTypeFromId( id ),
            name,
            Source( sourcePage ),
            rarity,
            restrictions ?? [],
            incompatibleChoiceIds ?? [],
            [ new AncestryEffectDescriptor( $"{id}.effect", effectKind, summary, visionOverride, baseHitPointsOverride ) ],
            dependencies );
    }

    private static AncestryFeat Feat(
        string id,
        string name,
        int sourcePage,
        string summary,
        IReadOnlyList<AncestryDependencyType> dependencies,
        AncestryEffectKind effectKind = AncestryEffectKind.RuleEffect,
        IReadOnlyList<string>? prerequisites = null,
        IReadOnlyList<string>? restrictions = null,
        IReadOnlyList<string>? incompatibleChoiceIds = null )
    {
        return new AncestryFeat(
            id,
            AncestryTypeFromId( id ),
            name,
            Source( sourcePage ),
            1,
            AncestryChoiceRarity.Common,
            prerequisites ?? [],
            restrictions ?? [],
            incompatibleChoiceIds ?? [],
            [ new AncestryEffectDescriptor( $"{id}.effect", effectKind, summary ) ],
            dependencies );
    }

    private static AncestryType AncestryTypeFromId( string id )
    {
        string ancestryName = id.Split( '.', 2 )[ 0 ];

        return ancestryName switch
        {
            "dwarf" => AncestryType.Dwarf,
            "elf" => AncestryType.Elf,
            "gnome" => AncestryType.Gnome,
            "goblin" => AncestryType.Goblin,
            "halfling" => AncestryType.Halfling,
            "human" => AncestryType.Human,
            _ => throw new ArgumentOutOfRangeException( nameof( id ), $"Ancestry id '{id}' is not supported." )
        };
    }
}
