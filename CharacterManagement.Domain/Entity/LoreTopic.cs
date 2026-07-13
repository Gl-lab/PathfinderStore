using System.Globalization;
using System.Text;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed record LoreTopic
{
    private const int MaxTopicLength = 100;

    public string Id { get; }
    public string Name { get; }

    private LoreTopic( string id, string name )
    {
        Id = id;
        Name = name;
    }

    public static LoreTopic CreateKnown( string id, string name )
    {
        if ( String.IsNullOrWhiteSpace( id ) || !id.StartsWith( "lore.", StringComparison.Ordinal ) )
        {
            throw new CharacterManagementException( "Lore id must use the 'lore.' prefix." );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new CharacterManagementException( "Lore name cannot be empty." );
        }

        return new LoreTopic( id.Trim(), NormalizeDisplayName( name ) );
    }

    public static LoreTopic CreateCustom(
        string topic,
        IReadOnlyCollection<SkillDefinition> generalSkills )
    {
        ArgumentNullException.ThrowIfNull( generalSkills );

        if ( String.IsNullOrWhiteSpace( topic ) )
        {
            throw new CharacterManagementException( "Custom Lore topic cannot be empty." );
        }

        string normalizedTopic = RemoveLoreSuffix( topic.Trim() );
        if ( String.IsNullOrWhiteSpace( normalizedTopic ) )
        {
            throw new CharacterManagementException( "Custom Lore topic cannot be empty." );
        }

        if ( normalizedTopic.Length > MaxTopicLength )
        {
            throw new CharacterManagementException(
                $"Custom Lore topic cannot exceed {MaxTopicLength} characters." );
        }

        if ( generalSkills.Any( skill =>
                String.Equals( skill.Name, normalizedTopic, StringComparison.OrdinalIgnoreCase ) ) )
        {
            throw new CharacterManagementException(
                $"Custom Lore cannot duplicate the general skill '{normalizedTopic}'." );
        }

        string canonicalKey = CreateCanonicalKey( normalizedTopic );
        if ( String.IsNullOrWhiteSpace( canonicalKey ) )
        {
            throw new CharacterManagementException( "Custom Lore topic must contain letters or digits." );
        }

        return new LoreTopic(
            $"lore.custom.{canonicalKey}",
            $"{normalizedTopic} Lore" );
    }

    private static string NormalizeDisplayName( string name )
    {
        string normalizedName = name.Trim();
        return normalizedName.EndsWith( " Lore", StringComparison.OrdinalIgnoreCase )
            ? normalizedName
            : $"{normalizedName} Lore";
    }

    private static string RemoveLoreSuffix( string topic )
    {
        if ( String.Equals( topic, "Lore", StringComparison.OrdinalIgnoreCase ) )
        {
            return String.Empty;
        }

        return topic.EndsWith( " Lore", StringComparison.OrdinalIgnoreCase )
            ? topic[ ..^5 ].Trim()
            : topic;
    }

    private static string CreateCanonicalKey( string topic )
    {
        string normalizedTopic = topic
            .Normalize( NormalizationForm.FormKC )
            .ToLower( CultureInfo.InvariantCulture );
        StringBuilder result = new StringBuilder();
        bool hasSeparator = false;

        foreach ( char character in normalizedTopic )
        {
            if ( Char.IsLetterOrDigit( character ) )
            {
                if ( hasSeparator && result.Length > 0 )
                {
                    result.Append( '_' );
                }

                result.Append( character );
                hasSeparator = false;
            }
            else
            {
                hasSeparator = result.Length > 0;
            }
        }

        return result.ToString();
    }
}
