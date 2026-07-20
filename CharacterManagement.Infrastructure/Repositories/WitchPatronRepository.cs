using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class WitchPatronRepository : IWitchPatronRepository
{
    private static readonly Dictionary<string, WitchPatron> WitchPatrons = CreateWitchPatrons()
        .ToDictionary( patron => patron.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<WitchPatron> GetAll() => WitchPatrons.Values.ToArray();

    public WitchPatron GetWitchPatron( string witchPatronId )
    {
        if ( String.IsNullOrWhiteSpace( witchPatronId ) )
        {
            throw new ArgumentException( "Witch Patron id cannot be empty.", nameof( witchPatronId ) );
        }

        if ( !WitchPatrons.TryGetValue( witchPatronId, out WitchPatron? patron ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( witchPatronId ),
                $"Witch Patron '{witchPatronId}' is not defined." );
        }

        return patron;
    }

    private static IReadOnlyCollection<WitchPatron> CreateWitchPatrons()
    {
        return
        [
            Create(
                "faiths_flamekeeper", "Faith's Flamekeeper", 184,
                SpellTradition.Divine, "religion",
                "fervors_grasp", "Fervor's Grasp",
                "stoke_the_heart", "Stoke the Heart",
                [ ( "command", "Command" ) ],
                "restored_spirit", "Restored Spirit",
                "Bolsters an ally when you cast or sustain a hex." ),
            Create(
                "inscribed_one", "The Inscribed One", 184,
                SpellTradition.Arcane, "arcana",
                "glyphs_supremacy", "Glyph's Supremacy",
                "discern_secrets", "Discern Secrets",
                [ ( "runic_weapon", "Runic Weapon" ) ],
                "flowing_script", "Flowing Script",
                "Lets the familiar provide flanking after a hex." ),
            Create(
                "resentment", "The Resentment", 184,
                SpellTradition.Occult, "occultism",
                "strengths_impermanence", "Strength's Impermanence",
                "evil_eye", "Evil Eye",
                [ ( "enfeeble", "Enfeeble" ) ],
                "ongoing_misery", "Ongoing Misery",
                "Can prolong a timed negative condition after a hex." ),
            Create(
                "silence_in_snow", "Silence in Snow", 185,
                SpellTradition.Primal, "nature",
                "winters_chill", "Winter's Chill",
                "clinging_ice", "Clinging Ice",
                [ ( "gust_of_wind", "Gust of Wind" ) ],
                "freezing_rime", "Freezing Rime",
                "Creates temporary difficult terrain around the familiar." ),
            Create(
                "spinner_of_threads", "Spinner of Threads", 185,
                SpellTradition.Occult, "occultism",
                "fates_vicissitudes", "Fate's Vicissitudes",
                "nudge_fate", "Nudge Fate",
                [ ( "sure_strike", "Sure Strike" ) ],
                "balanced_luck", "Balanced Luck",
                "Adjusts a nearby creature's AC after a hex." ),
            Create(
                "starless_shadow", "Starless Shadow", 185,
                SpellTradition.Occult, "occultism",
                "nights_terrors", "Night's Terrors",
                "shroud_of_night", "Shroud of Night",
                [ ( "fear", "Fear" ) ],
                "stalking_night", "Stalking Night",
                "Can frighten an enemy that cannot clearly perceive the familiar." ),
            Create(
                "wilding_steward", "Wilding Steward", 185,
                SpellTradition.Primal, "nature",
                "wild_speech", "Wild Speech",
                "wilding_word", "Wilding Word",
                [ ( "summon_animal", "Summon Animal" ), ( "summon_plant_or_fungus", "Summon Plant or Fungus" ) ],
                "keen_senses", "Keen Senses",
                "Grants the familiar a temporary imprecise sense and Point Out." ),
        ];
    }

    private static WitchPatron Create(
        string id,
        string name,
        int page,
        SpellTradition tradition,
        string skillId,
        string lessonId,
        string lessonName,
        string hexId,
        string hexName,
        IReadOnlyList<(string Id, string Name)> familiarSpells,
        string familiarAbilityId,
        string familiarAbilityName,
        string familiarAbilitySummary )
    {
        string patronId = $"witch_patron.{id}";
        return new WitchPatron(
            patronId,
            name,
            new SourceReference( "Player Core", page ),
            tradition,
            new ClassSkillGrantDescriptor( $"{patronId}.skill.patron", [ $"skill.{skillId}" ] ),
            [
                Benefit( $"lesson.{lessonId}", WitchPatronBenefitKind.Lesson, lessonName, "Defines the Patron's first lesson.", [ CharacterClassDependencyType.ClassFeatureRules ] ),
                Benefit( $"spell.{hexId}", WitchPatronBenefitKind.HexCantrip, hexName, "Granted hex cantrip.", [] ),
                .. familiarSpells.Select( spell => Benefit( $"spell.{spell.Id}", WitchPatronBenefitKind.FamiliarSpell, spell.Name, "Spell learned by the familiar.", [] ) ),
                Benefit( $"familiar_ability.{familiarAbilityId}", WitchPatronBenefitKind.FamiliarAbility, familiarAbilityName, familiarAbilitySummary, [ CharacterClassDependencyType.FamiliarRules ] ),
            ] );
    }

    private static WitchPatronBenefitDescriptor Benefit(
        string id,
        WitchPatronBenefitKind kind,
        string name,
        string summary,
        IReadOnlyList<CharacterClassDependencyType> dependencies )
    {
        return new WitchPatronBenefitDescriptor( id, kind, name, summary, dependencies );
    }
}
