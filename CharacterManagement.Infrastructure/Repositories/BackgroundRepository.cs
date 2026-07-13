using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class BackgroundRepository : IBackgroundRepository
{
    private static readonly Dictionary<string, Background> Backgrounds = CreateBackgrounds()
        .ToDictionary( background => background.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<Background> GetAll() => Backgrounds.Values.ToList();

    public Background GetBackground( string backgroundId )
    {
        if ( String.IsNullOrWhiteSpace( backgroundId ) )
        {
            throw new ArgumentException( "Background id cannot be empty.", nameof( backgroundId ) );
        }

        if ( !Backgrounds.TryGetValue( backgroundId, out Background? background ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( backgroundId ),
                $"Background '{backgroundId}' is not defined." );
        }

        return background;
    }

    private static IReadOnlyCollection<Background> CreateBackgrounds()
    {
        return
        [
            Create( "acolyte", "Acolyte", 60, AbilityType.Intelligence, AbilityType.Wisdom,
                Skill( "religion", "Religion" ), Lore( "scribing", "Scribing Lore" ), Feat( "student_of_the_canon", "Student of the Canon" ) ),
            Create( "acrobat", "Acrobat", 60, AbilityType.Strength, AbilityType.Dexterity,
                Skill( "acrobatics", "Acrobatics" ), Lore( "circus", "Circus Lore" ), Feat( "steady_balance", "Steady Balance" ) ),
            Create( "animal_whisperer", "Animal Whisperer", 60, AbilityType.Wisdom, AbilityType.Charisma,
                Skill( "nature", "Nature" ), Choice( "animal_whisperer.lore", BackgroundGrantKind.LoreTraining, "Animal terrain Lore", "Choose Lore related to terrain inhabited by favored animals." ), Feat( "train_animal", "Train Animal" ) ),
            Create( "artisan", "Artisan", 60, AbilityType.Strength, AbilityType.Intelligence,
                Skill( "crafting", "Crafting" ), Lore( "guild", "Guild Lore" ), Feat( "specialty_crafting", "Specialty Crafting" ) ),
            Create( "artist", "Artist", 60, AbilityType.Dexterity, AbilityType.Charisma,
                Skill( "crafting", "Crafting" ), Lore( "art", "Art Lore" ), Feat( "specialty_crafting", "Specialty Crafting" ) ),
            Create( "barkeep", "Barkeep", 60, AbilityType.Constitution, AbilityType.Charisma,
                Skill( "diplomacy", "Diplomacy" ), Lore( "alcohol", "Alcohol Lore" ), Feat( "hobnobber", "Hobnobber" ) ),
            Create( "barrister", "Barrister", 60, AbilityType.Intelligence, AbilityType.Charisma,
                Skill( "diplomacy", "Diplomacy" ), Lore( "legal", "Legal Lore" ), Feat( "group_impression", "Group Impression" ) ),
            Create( "bounty_hunter", "Bounty Hunter", 61, AbilityType.Strength, AbilityType.Wisdom,
                Skill( "survival", "Survival" ), Lore( "legal", "Legal Lore" ), Feat( "experienced_tracker", "Experienced Tracker" ) ),
            Create( "charlatan", "Charlatan", 61, AbilityType.Intelligence, AbilityType.Charisma,
                Skill( "deception", "Deception" ), Lore( "underworld", "Underworld Lore" ), Feat( "charming_liar", "Charming Liar" ) ),
            Create( "criminal", "Criminal", 61, AbilityType.Dexterity, AbilityType.Intelligence,
                Skill( "stealth", "Stealth" ), Lore( "underworld", "Underworld Lore" ), Feat( "experienced_smuggler", "Experienced Smuggler" ) ),
            Create( "detective", "Detective", 61, AbilityType.Intelligence, AbilityType.Wisdom,
                Skill( "society", "Society" ), Lore( "underworld", "Underworld Lore" ), Feat( "streetwise", "Streetwise" ) ),
            Create( "emissary", "Emissary", 61, AbilityType.Intelligence, AbilityType.Charisma,
                Skill( "society", "Society" ), Choice( "emissary.lore", BackgroundGrantKind.LoreTraining, "Visited city Lore", "Choose Lore related to a city visited often." ), Feat( "multilingual", "Multilingual" ) ),
            Create( "entertainer", "Entertainer", 61, AbilityType.Dexterity, AbilityType.Charisma,
                Skill( "performance", "Performance" ), Lore( "theater", "Theater Lore" ), Feat( "fascinating_performance", "Fascinating Performance" ) ),
            Create( "farmhand", "Farmhand", 62, AbilityType.Constitution, AbilityType.Wisdom,
                Skill( "athletics", "Athletics" ), Lore( "farming", "Farming Lore" ), Feat( "assurance", "Assurance" ) ),
            Create( "field_medic", "Field Medic", 62, AbilityType.Constitution, AbilityType.Wisdom,
                Skill( "medicine", "Medicine" ), Lore( "warfare", "Warfare Lore" ), Feat( "battle_medicine", "Battle Medicine" ) ),
            Create( "fortune_teller", "Fortune Teller", 62, AbilityType.Intelligence, AbilityType.Charisma,
                Skill( "occultism", "Occultism" ), Lore( "fortune_telling", "Fortune-Telling Lore" ), Feat( "oddity_identification", "Oddity Identification" ) ),
            Create( "gambler", "Gambler", 62, AbilityType.Dexterity, AbilityType.Charisma,
                Skill( "deception", "Deception" ), Lore( "games", "Games Lore" ), Feat( "lie_to_me", "Lie to Me" ) ),
            Create( "gladiator", "Gladiator", 62, AbilityType.Strength, AbilityType.Charisma,
                Skill( "performance", "Performance" ), Lore( "gladiatorial", "Gladiatorial Lore" ), Feat( "impressive_performance", "Impressive Performance" ) ),
            Create( "guard", "Guard", 62, AbilityType.Strength, AbilityType.Charisma,
                Skill( "intimidation", "Intimidation" ), Choice( "guard.lore", BackgroundGrantKind.LoreTraining, "Guard Lore", "Choose Legal Lore or Warfare Lore.", "lore.legal", "lore.warfare" ), Feat( "quick_coercion", "Quick Coercion" ) ),
            Create( "herbalist", "Herbalist", 62, AbilityType.Constitution, AbilityType.Wisdom,
                Skill( "nature", "Nature" ), Lore( "herbalism", "Herbalism Lore" ), Feat( "natural_medicine", "Natural Medicine" ) ),
            Create( "hermit", "Hermit", 62, AbilityType.Constitution, AbilityType.Intelligence,
                Choice( "hermit.skill", BackgroundGrantKind.SkillTraining, "Hermit skill", "Choose Nature or Occultism.", "skill.nature", "skill.occultism" ), Choice( "hermit.lore", BackgroundGrantKind.LoreTraining, "Hermit terrain Lore", "Choose Lore related to the terrain where the character lived." ), Feat( "dubious_knowledge", "Dubious Knowledge" ) ),
            Create( "hunter", "Hunter", 62, AbilityType.Dexterity, AbilityType.Wisdom,
                Skill( "survival", "Survival" ), Lore( "tanning", "Tanning Lore" ), Feat( "survey_wildlife", "Survey Wildlife" ) ),
            Create( "laborer", "Laborer", 62, AbilityType.Strength, AbilityType.Constitution,
                Skill( "athletics", "Athletics" ), Lore( "labor", "Labor Lore" ), Feat( "hefty_hauler", "Hefty Hauler" ) ),
            Create( "merchant", "Merchant", 63, AbilityType.Intelligence, AbilityType.Charisma,
                Skill( "diplomacy", "Diplomacy" ), Lore( "mercantile", "Mercantile Lore" ), Feat( "bargain_hunter", "Bargain Hunter" ) ),
            Create( "martial_disciple", "Martial Disciple", 63, AbilityType.Strength, AbilityType.Dexterity,
                Choice( "martial_disciple.skill", BackgroundGrantKind.SkillTraining, "Martial discipline skill", "Choose Acrobatics or Athletics.", "skill.acrobatics", "skill.athletics" ), Lore( "warfare", "Warfare Lore" ), Choice( "martial_disciple.feat", BackgroundGrantKind.SkillFeat, "Martial discipline feat", "Choose Cat Fall or Quick Jump.", "skill_feat.cat_fall", "skill_feat.quick_jump" ) ),
            Create( "miner", "Miner", 63, AbilityType.Strength, AbilityType.Wisdom,
                Skill( "survival", "Survival" ), Lore( "mining", "Mining Lore" ), Feat( "terrain_expertise", "Terrain Expertise" ) ),
            Create( "nomad", "Nomad", 63, AbilityType.Constitution, AbilityType.Wisdom,
                Skill( "survival", "Survival" ), Choice( "nomad.lore", BackgroundGrantKind.LoreTraining, "Traveled terrain Lore", "Choose Lore related to terrain traveled in." ), Feat( "assurance", "Assurance" ) ),
            Create( "noble", "Noble", 63, AbilityType.Intelligence, AbilityType.Charisma,
                Skill( "society", "Society" ), Choice( "noble.lore", BackgroundGrantKind.LoreTraining, "Noble Lore", "Choose Genealogy Lore or Heraldry Lore.", "lore.genealogy", "lore.heraldry" ), Feat( "courtly_graces", "Courtly Graces" ) ),
            Create( "prisoner", "Prisoner", 63, AbilityType.Strength, AbilityType.Constitution,
                Skill( "stealth", "Stealth" ), Lore( "underworld", "Underworld Lore" ), Feat( "experienced_smuggler", "Experienced Smuggler" ) ),
            Create( "sailor", "Sailor", 63, AbilityType.Strength, AbilityType.Dexterity,
                Skill( "athletics", "Athletics" ), Lore( "sailing", "Sailing Lore" ), Feat( "underwater_marauder", "Underwater Marauder" ) ),
            Create( "scholar", "Scholar", 63, AbilityType.Intelligence, AbilityType.Wisdom,
                Choice( "scholar.skill", BackgroundGrantKind.SkillTraining, "Scholarly skill", "Choose Arcana, Nature, Occultism, or Religion.", "skill.arcana", "skill.nature", "skill.occultism", "skill.religion" ), Lore( "academia", "Academia Lore" ), Feat( "assurance", "Assurance" ) ),
            Create( "scout", "Scout", 64, AbilityType.Dexterity, AbilityType.Wisdom,
                Skill( "survival", "Survival" ), Choice( "scout.lore", BackgroundGrantKind.LoreTraining, "Scouted terrain Lore", "Choose Lore related to terrain scouted." ), Feat( "forager", "Forager" ) ),
            Create( "street_urchin", "Street Urchin", 64, AbilityType.Dexterity, AbilityType.Constitution,
                Skill( "thievery", "Thievery" ), Choice( "street_urchin.lore", BackgroundGrantKind.LoreTraining, "Home city Lore", "Choose Lore related to the city where the character lived." ), Feat( "pickpocket", "Pickpocket" ) ),
            Create( "tinker", "Tinker", 64, AbilityType.Dexterity, AbilityType.Intelligence,
                Skill( "crafting", "Crafting" ), Lore( "engineering", "Engineering Lore" ), Feat( "specialty_crafting", "Specialty Crafting" ) ),
            Create( "warrior", "Warrior", 64, AbilityType.Strength, AbilityType.Constitution,
                Skill( "intimidation", "Intimidation" ), Lore( "warfare", "Warfare Lore" ), Feat( "intimidating_glare", "Intimidating Glare" ) ),
        ];
    }

    private static Background Create(
        string id,
        string name,
        int page,
        AbilityType firstRestrictedBoost,
        AbilityType secondRestrictedBoost,
        params BackgroundGrantDescriptor[] grants )
    {
        IReadOnlyList<BackgroundGrantDescriptor> normalizedGrants = grants
            .Select( grant => String.IsNullOrWhiteSpace( grant.Id )
                ? grant with { Id = $"background.{id}.{GetGrantSuffix( grant.Kind )}" }
                : grant )
            .ToList();

        return new Background(
            $"background.{id}",
            name,
            new SourceReference( "Core Rulebook", page ),
            [ firstRestrictedBoost, secondRestrictedBoost ],
            1,
            normalizedGrants );
    }

    private static string GetGrantSuffix( BackgroundGrantKind kind )
    {
        return kind switch
        {
            BackgroundGrantKind.SkillTraining => "skill",
            BackgroundGrantKind.LoreTraining => "lore",
            BackgroundGrantKind.SkillFeat => "skill_feat",
            _ => throw new ArgumentOutOfRangeException( nameof( kind ), kind, null )
        };
    }

    private static BackgroundGrantDescriptor Skill( string id, string name )
    {
        return Grant(
            $"skill.{id}",
            BackgroundGrantKind.SkillTraining,
            name,
            $"Grants training in {name}." );
    }

    private static BackgroundGrantDescriptor Lore( string id, string name )
    {
        return Grant(
            $"lore.{id}",
            BackgroundGrantKind.LoreTraining,
            name,
            $"Grants training in {name}." );
    }

    private static BackgroundGrantDescriptor Feat( string id, string name )
    {
        return Grant(
            $"skill_feat.{id}",
            BackgroundGrantKind.SkillFeat,
            name,
            $"Grants the {name} skill feat." );
    }

    private static BackgroundGrantDescriptor Grant(
        string targetId,
        BackgroundGrantKind kind,
        string name,
        string summary )
    {
        return new BackgroundGrantDescriptor(
            String.Empty,
            kind,
            name,
            summary,
            false,
            false,
            targetId,
            [],
            Dependencies( kind ) );
    }

    private static BackgroundGrantDescriptor Choice(
        string id,
        BackgroundGrantKind kind,
        string name,
        string summary,
        params string[] options )
    {
        return new BackgroundGrantDescriptor(
            $"background.{id}",
            kind,
            name,
            summary,
            true,
            kind == BackgroundGrantKind.LoreTraining && options.Length == 0,
            null,
            options
                .Select( option => new BackgroundGrantOption( option, GetOptionName( option ) ) )
                .ToList(),
            Dependencies( kind ) );
    }

    private static string GetOptionName( string optionId )
    {
        string value = optionId[( optionId.IndexOf( '.', StringComparison.Ordinal ) + 1 )..];
        string name = String.Join(
            " ",
            value
                .Split( '_', StringSplitOptions.RemoveEmptyEntries )
                .Select( part => Char.ToUpperInvariant( part[ 0 ] ) + part[ 1..] ) );
        return optionId.StartsWith( "lore.", StringComparison.Ordinal )
            ? $"{name} Lore"
            : name;
    }

    private static IReadOnlyList<BackgroundDependencyType> Dependencies( BackgroundGrantKind kind )
    {
        return kind switch
        {
            BackgroundGrantKind.SkillTraining =>
            [
                BackgroundDependencyType.ClassCatalog
            ],
            BackgroundGrantKind.LoreTraining =>
            [
                BackgroundDependencyType.ClassCatalog
            ],
            BackgroundGrantKind.SkillFeat => [ BackgroundDependencyType.SkillFeatCatalog ],
            _ => throw new ArgumentOutOfRangeException( nameof( kind ), kind, null )
        };
    }
}
