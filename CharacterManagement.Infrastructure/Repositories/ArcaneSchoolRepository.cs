using System.Globalization;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class ArcaneSchoolRepository : IArcaneSchoolRepository
{
    private static readonly Dictionary<string, ArcaneSchool> ArcaneSchools = CreateArcaneSchools()
        .ToDictionary( school => school.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<ArcaneSchool> GetAll() => ArcaneSchools.Values.ToArray();

    public ArcaneSchool GetArcaneSchool( string arcaneSchoolId )
    {
        if ( String.IsNullOrWhiteSpace( arcaneSchoolId ) )
        {
            throw new ArgumentException( "Arcane School id cannot be empty.", nameof( arcaneSchoolId ) );
        }

        if ( !ArcaneSchools.TryGetValue( arcaneSchoolId, out ArcaneSchool? school ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( arcaneSchoolId ),
                $"Arcane School '{arcaneSchoolId}' is not defined." );
        }

        return school;
    }

    private static IReadOnlyCollection<ArcaneSchool> CreateArcaneSchools()
    {
        return
        [
            Create(
                "ars_grammatica", "School of Ars Grammatica", 198,
                [
                    .. Rank( 0, "message", "sigil" ),
                    .. Rank( 1, "command", "disguise_magic", "runic_body", "runic_weapon" ),
                    .. Rank( 2, "dispel_magic", "translate" ),
                    Spell( 3, "enthrall" ), Spell( 3, "veil_of_privacy", true ),
                    Spell( 4, "dispelling_globe", true ), Spell( 4, "suggestion" ),
                    Spell( 5, "sending" ), Spell( 5, "truespeech", true ),
                    .. Rank( 6, "repulsion", "spellwrack" ),
                    Spell( 7, "contingency" ), Spell( 7, "planar_seal", true ),
                    .. Rank( 8, "quandary", "unrelenting_observation" ),
                    Spell( 9, "detonate_magic", true ),
                ],
                "protective_wards", "rune_of_observation" ),
            Create(
                "boundary", "School of the Boundary", 199,
                [
                    .. Rank( 0, "telekinetic_hand", "void_warp" ),
                    .. Rank( 1, "grim_tendrils", "phantasmal_minion", "summon_undead" ),
                    .. Rank( 2, "darkness", "see_the_unseen" ),
                    .. Rank( 3, "bind_undead", "ghostly_weapon" ),
                    .. Rank( 4, "flicker", "translocate" ),
                    .. Rank( 5, "banishment", "invoke_spirits" ),
                    Spell( 6, "teleport", true ), Spell( 6, "vampiric_exsanguination" ),
                    Spell( 7, "eclipse_burst" ), Spell( 7, "interplanar_teleport", true ),
                    .. Rank( 8, "quandary", "unrelenting_observation" ),
                    Spell( 9, "massacre" ),
                ],
                "fortify_summoning", "spiral_of_horrors" ),
            Create(
                "civic_wizardry", "School of Civic Wizardry", 199,
                [
                    .. Rank( 0, "prestidigitation", "read_aura" ),
                    .. Rank( 1, "hydraulic_push", "pummeling_rubble", "summon_construct" ),
                    .. Rank( 2, "revealing_light", "water_walk" ),
                    .. Rank( 3, "cozy_cabin", "safe_passage" ),
                    .. Rank( 4, "creation", "unfettered_movement" ),
                    .. Rank( 5, "control_water", "wall_of_stone" ),
                    .. Rank( 6, "disintegrate", "wall_of_force" ),
                    Spell( 7, "planar_palace", true ), Spell( 7, "retrocognition" ),
                    Spell( 8, "earthquake" ), Spell( 8, "pinpoint", true ),
                    Spell( 9, "foresight" ),
                ],
                "earthworks", "community_restoration" ),
            Create(
                "mentalism", "School of Mentalism", 200,
                [
                    .. Rank( 0, "daze", "figment" ),
                    .. Rank( 1, "dizzying_colors", "sleep", "sure_strike" ),
                    .. Rank( 2, "illusory_creature", "stupefy" ),
                    Spell( 3, "dream_message" ), Spell( 3, "mind_reading", true ),
                    .. Rank( 4, "nightmare", "vision_of_death" ),
                    .. Rank( 5, "hallucination", "illusory_scene" ),
                    .. Rank( 6, "never_mind", "phantasmal_calamity" ),
                    .. Rank( 7, "project_image", "warp_mind" ),
                    .. Rank( 8, "disappearance", "uncontrollable_dance" ),
                    Spell( 9, "phantasmagoria" ),
                ],
                "charming_push", "invisibility_cloak" ),
            Create(
                "protean_form", "School of Protean Form", 200,
                [
                    .. Rank( 0, "gouging_claw", "tangle_vine" ),
                    .. Rank( 1, "jump", "pest_form", "spider_sting" ),
                    .. Rank( 2, "enlarge", "humanoid_form" ),
                    .. Rank( 3, "feet_to_fins", "vampiric_feast" ),
                    .. Rank( 4, "mountain_resilience", "vapor_form" ),
                    .. Rank( 5, "elemental_form", "toxic_cloud" ),
                    .. Rank( 6, "cursed_metamorphosis", "petrify" ),
                    .. Rank( 7, "duplicate_foe", "fiery_body" ),
                    .. Rank( 8, "desiccate", "monstrosity_form" ),
                    Spell( 9, "metamorphosis" ),
                ],
                "scramble_body", "shifting_form" ),
            CreateUnifiedMagicalTheory(),
            Create(
                "battle_magic", "School of Battle Magic", 199,
                [
                    .. Rank( 0, "shield", "telekinetic_projectile" ),
                    .. Rank( 1, "breathe_fire", "force_barrage", "mystic_armor" ),
                    .. Rank( 2, "mist", "resist_energy" ),
                    .. Rank( 3, "earthbind", "fireball" ),
                    .. Rank( 4, "wall_of_fire", "weapon_storm" ),
                    .. Rank( 5, "howling_blizzard", "impaling_spike" ),
                    .. Rank( 6, "chain_lightning", "disintegrate" ),
                    .. Rank( 7, "energy_aegis", "true_target" ),
                    .. Rank( 8, "arctic_rift", "desiccate" ),
                    Spell( 9, "falling_stars" ),
                ],
                "force_bolt", "energy_absorption" ),
        ];
    }

    private static ArcaneSchool Create(
        string id,
        string name,
        int page,
        IReadOnlyList<ArcaneSchoolCurriculumSpellDescriptor> curriculum,
        string initialSchoolSpellId,
        string advancedSchoolSpellId )
    {
        return new ArcaneSchool(
            $"arcane_school.{id}",
            name,
            new SourceReference( "Player Core", page ),
            curriculum,
            [
                SchoolSpell( initialSchoolSpellId, ArcaneSchoolBenefitKind.InitialSchoolSpell ),
                SchoolSpell( advancedSchoolSpellId, ArcaneSchoolBenefitKind.AdvancedSchoolSpell ),
            ] );
    }

    private static ArcaneSchool CreateUnifiedMagicalTheory()
    {
        string schoolId = "arcane_school.unified_magical_theory";
        return new ArcaneSchool(
            schoolId,
            "School of Unified Magical Theory",
            new SourceReference( "Player Core", 200 ),
            [],
            [
                SchoolSpell( "hand_of_the_apprentice", ArcaneSchoolBenefitKind.InitialSchoolSpell ),
                SchoolSpell( "interdisciplinary_incantation", ArcaneSchoolBenefitKind.AdvancedSchoolSpell ),
                Benefit( schoolId, "extra_class_feat", ArcaneSchoolBenefitKind.ExtraClassFeat,
                    "Extra Wizard Class Feat", "Grants an additional 1st-level wizard class feat.",
                    CharacterClassDependencyType.ClassFeatCatalog ),
                Benefit( schoolId, "extra_spellbook_spell_choice", ArcaneSchoolBenefitKind.ExtraSpellbookSpellChoice,
                    "Extra Spellbook Spell", "Adds one 1st-rank spell of your choice to your spellbook." ),
                Benefit( schoolId, "additional_drain_bonded_item_uses", ArcaneSchoolBenefitKind.AdditionalDrainBondedItemUses,
                    "Expanded Drain Bonded Item", "Allows one Drain Bonded Item use per spell rank each day.",
                    CharacterClassDependencyType.ClassFeatureRules ),
            ] );
    }

    private static IReadOnlyList<ArcaneSchoolCurriculumSpellDescriptor> Rank(
        int rank,
        params string[] spellIds )
    {
        return spellIds
            .Select( id => Spell( rank, id ) )
            .ToArray();
    }

    private static ArcaneSchoolCurriculumSpellDescriptor Spell(
        int rank,
        string id,
        bool isUncommon = false )
    {
        return new ArcaneSchoolCurriculumSpellDescriptor(
            $"spell.{id}",
            CultureInfo.InvariantCulture.TextInfo.ToTitleCase( id.Replace( '_', ' ' ) ),
            rank,
            isUncommon );
    }

    private static ArcaneSchoolBenefitDescriptor SchoolSpell(
        string id,
        ArcaneSchoolBenefitKind kind )
    {
        return new ArcaneSchoolBenefitDescriptor(
            $"spell.{id}",
            kind,
            CultureInfo.InvariantCulture.TextInfo.ToTitleCase( id.Replace( '_', ' ' ) ),
            kind == ArcaneSchoolBenefitKind.InitialSchoolSpell
                ? "Initial school focus spell."
                : "Advanced school focus spell available through a later class feat.",
            kind == ArcaneSchoolBenefitKind.InitialSchoolSpell
                ? []
                : [ CharacterClassDependencyType.SpellCatalog, CharacterClassDependencyType.ClassFeatureRules ] );
    }

    private static ArcaneSchoolBenefitDescriptor Benefit(
        string schoolId,
        string id,
        ArcaneSchoolBenefitKind kind,
        string name,
        string summary,
        CharacterClassDependencyType? dependency = null )
    {
        return new ArcaneSchoolBenefitDescriptor(
            $"{schoolId}.benefit.{id}",
            kind,
            name,
            summary,
            dependency.HasValue ? [ dependency.Value ] : [] );
    }
}
