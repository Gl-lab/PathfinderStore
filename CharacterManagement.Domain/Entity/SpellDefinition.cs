namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum SpellRarity
{
    Common,
    Uncommon,
    Rare,
    Unique
}

public sealed class SpellDefinition
{
    public string Id { get; }
    public string Name { get; }
    public int Rank { get; }
    public SpellKind Kind { get; }
    public IReadOnlyList<SpellTradition> Traditions { get; }
    public IReadOnlyList<string> Traits { get; }
    public SpellRarity Rarity { get; }
    public SourceReference Source { get; }

    public SpellDefinition(
        string id,
        string name,
        int rank,
        SpellKind kind,
        IReadOnlyList<SpellTradition> traditions,
        IReadOnlyList<string> traits,
        SpellRarity rarity,
        SourceReference source )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "spell.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Spell id must use the 'spell.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Spell name cannot be empty.", nameof( name ) );
        }

        if ( rank < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( rank ), "Spell rank cannot be negative." );
        }

        ArgumentNullException.ThrowIfNull( traditions );
        ArgumentNullException.ThrowIfNull( traits );
        ArgumentNullException.ThrowIfNull( source );

        if ( traditions.Count == 0 )
        {
            throw new ArgumentException( "Spell must define at least one tradition.", nameof( traditions ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Rank = rank;
        Kind = kind;
        Traditions = traditions.Distinct().ToArray();
        Traits = traits
            .Where( trait => !String.IsNullOrWhiteSpace( trait ) )
            .Select( trait => trait.Trim() )
            .Distinct( StringComparer.Ordinal )
            .ToArray();
        Rarity = rarity;
        Source = source;
    }
}
