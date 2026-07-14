namespace Pathfinder.CharacterManagement.Domain.Entity;

public enum SpellKind
{
    Cantrip,
    Spell,
    Focus
}

public sealed class SpellReference
{
    public string Id { get; }
    public string Name { get; }
    public int Rank { get; }
    public SpellKind Kind { get; }

    public SpellReference( string id, string name, int rank, SpellKind kind )
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

        Id = id.Trim();
        Name = name.Trim();
        Rank = rank;
        Kind = kind;
    }
}

public sealed class ClericDomain
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public SpellReference InitialFocusSpell { get; }

    public ClericDomain(
        string id,
        string name,
        SourceReference source,
        SpellReference initialFocusSpell )
    {
        if ( String.IsNullOrWhiteSpace( id ) ||
             !id.StartsWith( "domain.", StringComparison.Ordinal ) )
        {
            throw new ArgumentException( "Cleric domain id must use the 'domain.' prefix.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Cleric domain name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( initialFocusSpell );

        if ( initialFocusSpell.Kind != SpellKind.Focus ||
             initialFocusSpell.Rank != 1 )
        {
            throw new ArgumentException(
                "A Cleric domain must define an initial rank-1 focus spell.",
                nameof( initialFocusSpell ) );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        InitialFocusSpell = initialFocusSpell;
    }
}
