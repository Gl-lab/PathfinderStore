using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class ClericDomainRepository : IClericDomainRepository
{
    private static readonly Dictionary<string, ClericDomain> _domains = CreateDomains()
        .ToDictionary( domain => domain.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<ClericDomain> GetAll() => _domains.Values
        .OrderBy( domain => domain.Name, StringComparer.Ordinal )
        .ToArray();

    public ClericDomain GetClericDomain( string clericDomainId )
    {
        if ( String.IsNullOrWhiteSpace( clericDomainId ) )
        {
            throw new ArgumentException( "Cleric domain id cannot be empty.", nameof( clericDomainId ) );
        }

        if ( !_domains.TryGetValue( clericDomainId, out ClericDomain? clericDomain ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( clericDomainId ),
                $"Cleric domain '{clericDomainId}' is not defined." );
        }

        return clericDomain;
    }

    private static IReadOnlyCollection<ClericDomain> CreateDomains()
    {
        return
        [
            Create( "Air", "Pushing Gust" ),
            Create( "Ambition", "Ignite Ambition" ),
            Create( "Cities", "Face in the Crowd" ),
            Create( "Confidence", "Veil of Confidence" ),
            Create( "Creation", "Creative Splash" ),
            Create( "Darkness", "Cloak of Shadow" ),
            Create( "Death", "Death's Call" ),
            Create( "Destruction", "Cry of Destruction" ),
            Create( "Dreams", "Sweet Dream" ),
            Create( "Earth", "Hurtling Stone" ),
            Create( "Family", "Soothing Words" ),
            Create( "Fate", "Read Fate" ),
            Create( "Fire", "Fire Ray" ),
            Create( "Freedom", "Unimpeded Stride" ),
            Create( "Healing", "Healer's Blessing" ),
            Create( "Indulgence", "Overstuff" ),
            Create( "Knowledge", "Scholarly Recollection" ),
            Create( "Luck", "Bit of Luck" ),
            Create( "Magic", "Magic's Vessel" ),
            Create( "Metal", "Serrate" ),
            Create( "Might", "Athletic Rush" ),
            Create( "Moon", "Moonbeam" ),
            Create( "Nature", "Vibrant Thorns" ),
            Create( "Nightmares", "Waking Nightmare" ),
            Create( "Pain", "Savor the Sting" ),
            Create( "Passion", "Charming Touch" ),
            Create( "Perfection", "Perfected Mind" ),
            Create( "Protection", "Protector's Sacrifice" ),
            Create( "Secrecy", "Whispering Quiet" ),
            Create( "Sun", "Dazzling Flash" ),
            Create( "Travel", "Agile Feet" ),
            Create( "Trickery", "Sudden Shift" ),
            Create( "Truth", "Word of Truth" ),
            Create( "Tyranny", "Touch of Obedience" ),
            Create( "Undeath", "Touch of Undeath" ),
            Create( "Water", "Tidal Surge" ),
            Create( "Wealth", "Appearance of Wealth" ),
            Create( "Wood", "Arms of Nature" ),
            Create( "Zeal", "Weapon Surge" ),
        ];
    }

    private static ClericDomain Create( string name, string initialFocusSpellName )
    {
        return new ClericDomain(
            $"domain.{Slug( name )}",
            name,
            new SourceReference( "Player Core", 35 ),
            new SpellReference(
                $"spell.{Slug( initialFocusSpellName )}",
                initialFocusSpellName,
                1,
                SpellKind.Focus ) );
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
