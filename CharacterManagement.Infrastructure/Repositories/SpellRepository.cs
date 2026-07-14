using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class SpellRepository : ISpellRepository
{
    private static readonly Dictionary<string, SpellDefinition> _spells = CreateSpells()
        .ToDictionary( spell => spell.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<SpellDefinition> GetAll() => _spells.Values
        .OrderBy( spell => spell.Kind )
        .ThenBy( spell => spell.Name, StringComparer.Ordinal )
        .ToArray();

    public SpellDefinition GetSpell( string spellId )
    {
        if ( String.IsNullOrWhiteSpace( spellId ) )
        {
            throw new ArgumentException( "Spell id cannot be empty.", nameof( spellId ) );
        }

        if ( !_spells.TryGetValue( spellId, out SpellDefinition? spell ) )
        {
            throw new ArgumentOutOfRangeException( nameof( spellId ), $"Spell '{spellId}' is not defined." );
        }

        return spell;
    }

    private static IReadOnlyCollection<SpellDefinition> CreateSpells()
    {
        return
        [
            Create( "Daze", SpellKind.Cantrip, "ADO", "Cantrip|Concentrate|Manipulate|Mental|Nonlethal", SpellRarity.Common, "Player Core", 322 ),
            Create( "Detect Magic", SpellKind.Cantrip, "ADOP", "Cantrip|Concentrate|Detection|Manipulate", SpellRarity.Common, "Player Core", 323 ),
            Create( "Divine Lance", SpellKind.Cantrip, "D", "Attack|Cantrip|Concentrate|Manipulate|Sanctified|Spirit", SpellRarity.Common, "Player Core", 325 ),
            Create( "Forbidding Ward", SpellKind.Cantrip, "DO", "Cantrip|Concentrate|Manipulate", SpellRarity.Common, "Player Core", 332 ),
            Create( "Guidance", SpellKind.Cantrip, "DOP", "Cantrip|Concentrate", SpellRarity.Common, "Player Core", 334 ),
            Create( "Know the Way", SpellKind.Cantrip, "DOP", "Cantrip|Concentrate|Detection|Manipulate", SpellRarity.Common, "Player Core", 340 ),
            Create( "Light", SpellKind.Cantrip, "ADOP", "Cantrip|Concentrate|Light|Manipulate", SpellRarity.Common, "Player Core", 340 ),
            Create( "Message", SpellKind.Cantrip, "ADO", "Auditory|Cantrip|Concentrate|Illusion|Linguistic|Mental|Subtle", SpellRarity.Common, "Player Core", 343 ),
            Create( "Prestidigitation", SpellKind.Cantrip, "ADOP", "Cantrip|Concentrate|Manipulate", SpellRarity.Common, "Player Core", 351 ),
            Create( "Read Aura", SpellKind.Cantrip, "ADOP", "Cantrip|Concentrate|Detection|Manipulate", SpellRarity.Common, "Player Core", 352 ),
            Create( "Shield", SpellKind.Cantrip, "ADO", "Cantrip|Concentrate|Force", SpellRarity.Common, "Player Core", 356 ),
            Create( "Sigil", SpellKind.Cantrip, "ADOP", "Cantrip|Concentrate|Manipulate", SpellRarity.Common, "Player Core", 357 ),
            Create( "Stabilize", SpellKind.Cantrip, "DP", "Cantrip|Concentrate|Healing|Manipulate|Vitality", SpellRarity.Common, "Player Core", 359 ),
            Create( "Summon Instrument", SpellKind.Cantrip, "ADO", "Cantrip|Concentrate|Manipulate", SpellRarity.Common, "Player Core", 361 ),
            Create( "Vitality Lash", SpellKind.Cantrip, "DP", "Cantrip|Concentrate|Manipulate|Vitality", SpellRarity.Common, "Player Core", 366 ),
            Create( "Void Warp", SpellKind.Cantrip, "ADO", "Cantrip|Concentrate|Manipulate|Void", SpellRarity.Common, "Player Core", 366 ),

            Create( "Air Bubble", SpellKind.Spell, "ADP", "Air|Concentrate", SpellRarity.Common, "Player Core", 314 ),
            Create( "Alarm", SpellKind.Spell, "ADOP", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 314 ),
            Create( "Bane", SpellKind.Spell, "DO", "Aura|Concentrate|Manipulate|Mental", SpellRarity.Common, "Player Core", 317 ),
            Create( "Bless", SpellKind.Spell, "DO", "Aura|Concentrate|Manipulate|Mental", SpellRarity.Common, "Player Core", 318 ),
            Create( "Cleanse Cuisine", SpellKind.Spell, "DP", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 320 ),
            Create( "Command", SpellKind.Spell, "ADO", "Auditory|Concentrate|Linguistic|Manipulate|Mental", SpellRarity.Common, "Player Core", 321 ),
            Create( "Create Water", SpellKind.Spell, "ADP", "Concentrate|Manipulate|Water", SpellRarity.Common, "Player Core", 322 ),
            Create( "Enfeeble", SpellKind.Spell, "ADO", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 329 ),
            Create( "Fear", SpellKind.Spell, "ADOP", "Concentrate|Emotion|Fear|Manipulate|Mental", SpellRarity.Common, "Player Core", 331 ),
            Create( "Harm", SpellKind.Spell, "D", "Manipulate|Void", SpellRarity.Common, "Player Core", 334 ),
            Create( "Heal", SpellKind.Spell, "DP", "Healing|Manipulate|Vitality", SpellRarity.Common, "Player Core", 335 ),
            Create( "Infuse Vitality", SpellKind.Spell, "D", "Concentrate|Manipulate|Vitality", SpellRarity.Common, "Player Core", 338 ),
            Create( "Lock", SpellKind.Spell, "ADO", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 341 ),
            Create( "Mending", SpellKind.Spell, "ADOP", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 343 ),
            Create( "Mystic Armor", SpellKind.Spell, "ADOP", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 346 ),
            Create( "Pet Cache", SpellKind.Spell, "ADOP", "Extradimensional|Manipulate", SpellRarity.Common, "Player Core", 348 ),
            Create( "Protection", SpellKind.Spell, "DO", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 351 ),
            Create( "Runic Body", SpellKind.Spell, "ADOP", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 354 ),
            Create( "Runic Weapon", SpellKind.Spell, "ADOP", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 354 ),
            Create( "Sanctuary", SpellKind.Spell, "DO", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 355 ),
            Create( "Spirit Link", SpellKind.Spell, "DO", "Concentrate|Healing|Manipulate|Spirit", SpellRarity.Common, "Player Core", 358 ),
            Create( "Summon Undead", SpellKind.Spell, "ADO", "Concentrate|Manipulate|Summon", SpellRarity.Common, "Player Core", 361 ),
            Create( "Ventriloquism", SpellKind.Spell, "ADOP", "Auditory|Concentrate|Illusion|Manipulate", SpellRarity.Common, "Player Core", 366 ),

            Create( "Breathe Fire", SpellKind.Spell, "AP", "Concentrate|Fire|Manipulate", SpellRarity.Common, "Player Core", 319 ),
            Create( "Charm", SpellKind.Spell, "AOP", "Concentrate|Emotion|Incapacitation|Manipulate|Mental|Subtle", SpellRarity.Common, "Player Core", 320 ),
            Create( "Dizzying Colors", SpellKind.Spell, "AO", "Concentrate|Illusion|Incapacitation|Manipulate|Visual", SpellRarity.Common, "Player Core", 326 ),
            Create( "Fleet Step", SpellKind.Spell, "AP", "Concentrate|Manipulate", SpellRarity.Common, "Player Core", 332 ),
            Create( "Force Barrage", SpellKind.Spell, "AO", "Concentrate|Force|Manipulate", SpellRarity.Common, "Player Core", 332 ),
            Create( "Goblin Pox", SpellKind.Spell, "AP", "Concentrate|Disease|Manipulate", SpellRarity.Common, "Player Core", 333 ),
            Create( "Gust of Wind", SpellKind.Spell, "AP", "Air|Concentrate|Manipulate", SpellRarity.Common, "Player Core", 334 ),
            Create( "Illusory Disguise", SpellKind.Spell, "AO", "Concentrate|Illusion|Manipulate|Visual", SpellRarity.Common, "Player Core", 337 ),
            Create( "Illusory Object", SpellKind.Spell, "AO", "Concentrate|Illusion|Manipulate|Visual", SpellRarity.Common, "Player Core", 337 ),
            Create( "Jump", SpellKind.Spell, "AP", "Manipulate|Move", SpellRarity.Common, "Player Core", 340 ),
            Create( "Mindlink", SpellKind.Spell, "AO", "Concentrate|Manipulate|Mental", SpellRarity.Common, "Player Core", 344 ),
            Create( "Phantom Pain", SpellKind.Spell, "O", "Concentrate|Illusion|Manipulate|Mental|Nonlethal", SpellRarity.Common, "Player Core", 349 ),
            Create( "Sleep", SpellKind.Spell, "AO", "Concentrate|Incapacitation|Manipulate|Mental|Sleep", SpellRarity.Common, "Player Core", 357 ),
            Create( "Spider Sting", SpellKind.Spell, "AP", "Concentrate|Manipulate|Poison", SpellRarity.Common, "Player Core", 358 ),
            Create( "Summon Animal", SpellKind.Spell, "AP", "Concentrate|Manipulate|Summon", SpellRarity.Common, "Player Core", 360 ),
            Create( "Sure Strike", SpellKind.Spell, "AO", "Concentrate|Fortune", SpellRarity.Common, "Player Core", 361 ),

            Focus( "Pushing Gust", "Air|Cleric|Concentrate|Focus|Manipulate|Uncommon", "Player Core", 372 ),
            Focus( "Ignite Ambition", "Cleric|Concentrate|Emotion|Focus|Mental|Subtle|Uncommon", "Player Core", 372 ),
            Focus( "Face in the Crowd", "Cleric|Focus|Manipulate|Uncommon|Visual", "Player Core", 372 ),
            Focus( "Veil of Confidence", "Cleric|Concentrate|Focus|Mental|Uncommon", "Player Core", 373 ),
            Focus( "Creative Splash", "Cleric|Concentrate|Focus|Illusion|Manipulate|Uncommon|Visual", "Player Core", 373 ),
            Focus( "Cloak of Shadow", "Aura|Cleric|Darkness|Focus|Manipulate|Shadow|Uncommon", "Player Core", 373 ),
            Focus( "Death's Call", "Cleric|Concentrate|Focus|Uncommon", "Player Core", 373 ),
            Focus( "Cry of Destruction", "Cleric|Concentrate|Focus|Manipulate|Sonic|Uncommon", "Player Core", 374 ),
            Focus( "Sweet Dream", "Auditory|Cleric|Concentrate|Focus|Linguistic|Manipulate|Mental|Sleep|Uncommon", "Player Core", 374 ),
            Focus( "Hurtling Stone", "Attack|Cleric|Earth|Focus|Manipulate|Uncommon", "Player Core", 374 ),
            Focus( "Soothing Words", "Cleric|Concentrate|Emotion|Focus|Mental|Uncommon", "Player Core", 374 ),
            Focus( "Read Fate", "Cleric|Concentrate|Focus|Manipulate|Prediction|Uncommon", "Player Core", 375 ),
            Focus( "Fire Ray", "Attack|Cleric|Concentrate|Fire|Focus|Manipulate|Uncommon", "Player Core", 375 ),
            Focus( "Unimpeded Stride", "Cleric|Focus|Manipulate|Uncommon", "Player Core", 375 ),
            Focus( "Healer's Blessing", "Cleric|Concentrate|Focus|Uncommon", "Player Core", 375 ),
            Focus( "Overstuff", "Cleric|Concentrate|Focus|Manipulate|Uncommon", "Player Core", 376 ),
            Focus( "Scholarly Recollection", "Cleric|Focus|Fortune|Uncommon", "Player Core", 376 ),
            Focus( "Bit of Luck", "Cleric|Concentrate|Focus|Fortune|Manipulate|Uncommon", "Player Core", 376 ),
            Focus( "Magic's Vessel", "Cleric|Focus|Manipulate|Uncommon", "Player Core", 376 ),
            Focus( "Serrate", "Cleric|Focus|Manipulate|Metal|Uncommon", "Divine Mysteries", 267 ),
            Focus( "Athletic Rush", "Cleric|Focus|Manipulate|Uncommon", "Player Core", 377 ),
            Focus( "Moonbeam", "Attack|Cleric|Concentrate|Fire|Focus|Light|Manipulate|Uncommon", "Player Core", 377 ),
            Focus( "Vibrant Thorns", "Cleric|Focus|Manipulate|Morph|Plant|Uncommon|Wood", "Player Core", 377 ),
            Focus( "Waking Nightmare", "Cleric|Concentrate|Emotion|Fear|Focus|Manipulate|Mental|Uncommon", "Player Core", 377 ),
            Focus( "Savor the Sting", "Cleric|Focus|Manipulate|Mental|Nonlethal|Uncommon", "Player Core", 378 ),
            Focus( "Charming Touch", "Cleric|Emotion|Focus|Incapacitation|Manipulate|Mental|Subtle|Uncommon", "Player Core", 378 ),
            Focus( "Perfected Mind", "Cleric|Concentrate|Focus|Uncommon", "Player Core", 378 ),
            Focus( "Protector's Sacrifice", "Cleric|Focus|Manipulate|Uncommon", "Player Core", 378 ),
            Focus( "Whispering Quiet", "Cleric|Focus|Manipulate|Sonic|Uncommon", "Player Core", 379 ),
            Focus( "Dazzling Flash", "Cleric|Concentrate|Focus|Light|Manipulate|Uncommon|Visual", "Player Core", 379 ),
            Focus( "Agile Feet", "Cleric|Focus|Manipulate|Uncommon", "Player Core", 379 ),
            Focus( "Sudden Shift", "Cleric|Focus|Manipulate|Uncommon", "Player Core", 379 ),
            Focus( "Word of Truth", "Cleric|Concentrate|Focus|Uncommon", "Player Core", 380 ),
            Focus( "Touch of Obedience", "Cleric|Focus|Manipulate|Mental|Uncommon", "Player Core", 380 ),
            Focus( "Touch of Undeath", "Cleric|Focus|Manipulate|Uncommon|Void", "Player Core", 380 ),
            Focus( "Tidal Surge", "Cleric|Focus|Manipulate|Uncommon|Water", "Player Core", 381 ),
            Focus( "Appearance of Wealth", "Cleric|Concentrate|Focus|Illusion|Manipulate|Uncommon", "Player Core", 381 ),
            Focus( "Arms of Nature", "Cleric|Concentrate|Focus|Manipulate|Uncommon|Wood", "Divine Mysteries", 271 ),
            Focus( "Weapon Surge", "Cleric|Focus|Manipulate|Sanctified|Uncommon", "Player Core", 381 ),
        ];
    }

    private static SpellDefinition Focus( string name, string traits, string book, int page ) =>
        Create( name, SpellKind.Focus, "D", traits, SpellRarity.Uncommon, book, page );

    private static SpellDefinition Create(
        string name,
        SpellKind kind,
        string traditionCodes,
        string traits,
        SpellRarity rarity,
        string book,
        int page )
    {
        return new SpellDefinition(
            $"spell.{Slug( name )}",
            name,
            1,
            kind,
            ParseTraditions( traditionCodes ),
            traits.Split( '|', StringSplitOptions.RemoveEmptyEntries ),
            rarity,
            new SourceReference( book, page ) );
    }

    private static IReadOnlyList<SpellTradition> ParseTraditions( string codes )
    {
        return codes.Select( code => code switch
        {
            'A' => SpellTradition.Arcane,
            'D' => SpellTradition.Divine,
            'O' => SpellTradition.Occult,
            'P' => SpellTradition.Primal,
            _ => throw new ArgumentOutOfRangeException( nameof( codes ), $"Unknown tradition code '{code}'." ),
        } ).ToArray();
    }

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
