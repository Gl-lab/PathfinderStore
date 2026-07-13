using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class DeityRepository : IDeityRepository
{
    private static readonly Dictionary<string, Deity> Deities = CreateDeities()
        .ToDictionary( deity => deity.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<Deity> GetAll() => Deities.Values.ToArray();

    public Deity GetDeity( string deityId )
    {
        if ( String.IsNullOrWhiteSpace( deityId ) )
        {
            throw new ArgumentException( "Deity id cannot be empty.", nameof( deityId ) );
        }

        if ( !Deities.TryGetValue( deityId, out Deity? deity ) )
        {
            throw new ArgumentOutOfRangeException( nameof( deityId ), $"Deity '{deityId}' is not defined." );
        }

        return deity;
    }

    private static IReadOnlyCollection<Deity> CreateDeities()
    {
        return
        [
            Create( "abadar", "Abadar", "society", [ "Crossbow" ], [ DivineFont.Harm, DivineFont.Heal ], Both(), null, [ "Cities", "Earth", "Travel", "Wealth" ], [ S( 1, "Illusory Object" ), S( 4, "Creation" ), S( 7, "Planar Palace" ) ] ),
            Create( "asmodeus", "Asmodeus", "deception", [ "Mace" ], [ DivineFont.Harm ], [ DivineSanctification.Unholy ], DivineSanctification.Unholy, [ "Confidence", "Fire", "Trickery", "Tyranny" ], [ S( 1, "Charm" ), S( 4, "Suggestion" ), S( 6, "Mislead" ) ] ),
            Create( "calistria", "Calistria", "deception", [ "Whip" ], [ DivineFont.Harm, DivineFont.Heal ], Both(), null, [ "Pain", "Passion", "Secrecy", "Trickery" ], [ S( 1, "Charm" ), S( 3, "Enthrall" ), S( 6, "Mislead" ) ] ),
            Create( "cayden_cailean", "Cayden Cailean", "athletics", [ "Rapier" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], null, [ "Cities", "Freedom", "Indulgence", "Might" ], [ S( 1, "Fleet Step" ), S( 2, "Stupefy" ), S( 5, "Hallucination" ) ] ),
            Create( "desna", "Desna", "acrobatics", [ "Starknife" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], null, [ "Dreams", "Luck", "Moon", "Travel" ], [ S( 1, "Sleep" ), S( 4, "Translocate" ), S( 5, "Dreaming Potential" ) ] ),
            Create( "erastil", "Erastil", "survival", [ "Longbow" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], null, [ "Earth", "Family", "Nature", "Wealth" ], [ S( 1, "Sure Strike" ), S( 3, "Wall of Thorns" ), S( 5, "Nature's Pathway" ) ] ),
            Create( "gorum", "Gorum", "athletics", [ "Greatsword" ], [ DivineFont.Harm, DivineFont.Heal ], Both(), null, [ "Confidence", "Destruction", "Might", "Zeal" ], [ S( 1, "Sure Strike" ), S( 2, "Enlarge" ), S( 4, "Weapon Storm" ) ] ),
            Create( "gozreh", "Gozreh", "survival", [ "Trident" ], [ DivineFont.Heal ], [], null, [ "Air", "Nature", "Travel", "Water" ], [ S( 1, "Gust of Wind" ), S( 3, "Lightning Bolt" ), S( 5, "Control Water" ) ] ),
            Create( "iomedae", "Iomedae", "intimidation", [ "Longsword" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], DivineSanctification.Holy, [ "Confidence", "Might", "Truth", "Zeal" ], [ S( 1, "Sure Strike" ), S( 2, "Enlarge" ), S( 4, "Fire Shield" ) ] ),
            Create( "irori", "Irori", "athletics", [ "Fist" ], [ DivineFont.Harm, DivineFont.Heal ], Both(), null, [ "Knowledge", "Might", "Perfection", "Truth" ], [ S( 1, "Jump" ), S( 3, "Haste" ), S( 4, "Mountain Resilience" ) ] ),
            Create( "lamashtu", "Lamashtu", "survival", [ "Falchion" ], [ DivineFont.Harm, DivineFont.Heal ], [ DivineSanctification.Unholy ], null, [ "Family", "Might", "Nightmares", "Trickery" ], [ S( 1, "Spider Sting" ), S( 2, "Animal Form" ), S( 4, "Nightmare" ) ] ),
            Create( "nethys", "Nethys", "arcana", [ "Staff" ], [ DivineFont.Harm, DivineFont.Heal ], Both(), null, [ "Destruction", "Knowledge", "Magic", "Protection" ], [ S( 1, "Force Barrage" ), S( 2, "Embed Message" ), S( 3, "Levitate" ), S( 4, "Flicker" ), S( 5, "Telekinetic Haul" ), S( 6, "Wall of Force" ), S( 7, "Warp Mind" ), S( 8, "Quandary" ), S( 9, "Detonate Magic" ) ] ),
            Create( "norgorber", "Norgorber", "stealth", [ "Shortsword" ], [ DivineFont.Harm ], [ DivineSanctification.Unholy ], null, [ "Death", "Secrecy", "Trickery", "Wealth" ], [ S( 1, "Illusory Disguise" ), S( 2, "Invisibility" ), S( 4, "Vision of Death" ) ] ),
            Create( "pharasma", "Pharasma", "medicine", [ "Dagger" ], [ DivineFont.Heal ], [], null, [ "Death", "Fate", "Healing", "Knowledge" ], [ S( 1, "Mindlink" ), S( 3, "Ghostly Weapon" ), S( 4, "Vision of Death" ) ] ),
            Create( "rovagug", "Rovagug", "athletics", [ "Greataxe" ], [ DivineFont.Harm ], [ DivineSanctification.Unholy ], DivineSanctification.Unholy, [ "Air", "Destruction", "Earth", "Zeal" ], [ S( 1, "Breathe Fire" ), S( 2, "Enlarge" ), S( 6, "Disintegrate" ) ] ),
            Create( "sarenrae", "Sarenrae", "medicine", [ "Scimitar" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], null, [ "Fire", "Healing", "Sun", "Truth" ], [ S( 1, "Breathe Fire" ), S( 3, "Fireball" ), S( 4, "Wall of Fire" ) ] ),
            Create( "shelyn", "Shelyn", "crafting", [ "Glaive" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], null, [ "Creation", "Family", "Passion", "Protection" ], [ S( 1, "Dizzying Colors" ), S( 3, "Enthrall" ), S( 4, "Creation" ) ] ),
            Create( "torag", "Torag", "crafting", [ "Warhammer" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], null, [ "Creation", "Earth", "Family", "Protection" ], [ S( 1, "Mindlink" ), S( 3, "Earthbind" ), S( 4, "Creation" ) ] ),
            Create( "urgathoa", "Urgathoa", "intimidation", [ "Scythe" ], [ DivineFont.Harm ], [ DivineSanctification.Unholy ], DivineSanctification.Unholy, [ "Indulgence", "Magic", "Might", "Undeath" ], [ S( 1, "Goblin Pox" ), S( 2, "False Vitality" ), S( 7, "Mask of Terror" ) ] ),
            Create( "zon_kuthon", "Zon-Kuthon", "intimidation", [ "Spiked Chain" ], [ DivineFont.Harm ], [ DivineSanctification.Unholy ], null, [ "Ambition", "Darkness", "Destruction", "Pain" ], [ S( 1, "Phantom Pain" ), S( 3, "Wall of Thorns" ), S( 5, "Umbral Journey" ) ] ),
            Create( "green_faith", "Green Faith", "nature", [ "Sickle", "Claw" ], [ DivineFont.Heal ], [ DivineSanctification.Holy ], null, [ "Air", "Earth", "Fire", "Metal", "Nature", "Water", "Wood" ], [ S( 1, "Summon Animal" ), S( 2, "Speak with Animals" ), S( 3, "Wall of Thorns" ) ] ),
            new Deity( "deity.atheism", "Atheism", new SourceReference( "Player Core", 39 ), false, null, [], [], [], null, [], [] ),
        ];
    }

    private static Deity Create(
        string id,
        string name,
        string skillId,
        IReadOnlyList<string> weaponNames,
        IReadOnlyList<DivineFont> fonts,
        IReadOnlyList<DivineSanctification> sanctifications,
        DivineSanctification? requiredSanctification,
        IReadOnlyList<string> domains,
        IReadOnlyList<DeityGrantedSpell> spells )
    {
        return new Deity(
            $"deity.{id}",
            name,
            new SourceReference( "Player Core", 35 ),
            true,
            $"skill.{skillId}",
            weaponNames.Select( Weapon ).ToArray(),
            fonts,
            sanctifications,
            requiredSanctification,
            domains.Select( domain => $"domain.{Slug( domain )}" ).ToArray(),
            spells );
    }

    private static DeityFavoredWeapon Weapon( string name )
    {
        FavoredWeaponCategory category = name switch
        {
            "Crossbow" or "Mace" or "Staff" or "Dagger" or "Sickle" => FavoredWeaponCategory.Simple,
            "Fist" or "Claw" => FavoredWeaponCategory.Unarmed,
            _ => FavoredWeaponCategory.Martial,
        };
        return new DeityFavoredWeapon( $"weapon.{Slug( name )}", name, category );
    }

    private static DeityGrantedSpell S( int rank, string name ) =>
        new DeityGrantedSpell( rank, $"spell.{Slug( name )}", name );

    private static IReadOnlyList<DivineSanctification> Both() =>
        [ DivineSanctification.Holy, DivineSanctification.Unholy ];

    private static string Slug( string value )
    {
        return new string( value
                .ToLowerInvariant()
                .Select( character => Char.IsLetterOrDigit( character ) ? character : '_' )
                .ToArray() )
            .Replace( "__", "_", StringComparison.Ordinal )
            .Trim( '_' );
    }
}
