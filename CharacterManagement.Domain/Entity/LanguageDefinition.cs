namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum LanguageRarity
{
    Common,
    Uncommon
}

public enum LanguageCategory
{
    Standard,
    Regional
}

public sealed class LanguageDefinition
{
    public LanguageId Id { get; }
    public string Name { get; }
    public string Speakers { get; }
    public LanguageRarity Rarity { get; }
    public LanguageCategory Category { get; }
    public SourceReference Source { get; }

    public LanguageDefinition(
        LanguageId id,
        string name,
        string speakers,
        LanguageRarity rarity,
        LanguageCategory category,
        SourceReference source )
    {
        if ( String.IsNullOrWhiteSpace( id.Value )
            || !id.Value.All( character => ( character >= 'a' ) && ( character <= 'z' ) ) )
        {
            throw new ArgumentException(
                "Language id must contain only lowercase ASCII letters.",
                nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Language name cannot be empty.", nameof( name ) );
        }

        if ( String.IsNullOrWhiteSpace( speakers ) )
        {
            throw new ArgumentException( "Language speakers cannot be empty.", nameof( speakers ) );
        }

        if ( !Enum.IsDefined( rarity ) )
        {
            throw new ArgumentOutOfRangeException( nameof( rarity ), rarity, null );
        }

        if ( !Enum.IsDefined( category ) )
        {
            throw new ArgumentOutOfRangeException( nameof( category ), category, null );
        }

        ArgumentNullException.ThrowIfNull( source );

        Id = id;
        Name = name.Trim();
        Speakers = speakers.Trim();
        Rarity = rarity;
        Category = category;
        Source = source;
    }
}
