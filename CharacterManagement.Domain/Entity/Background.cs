namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed class Background
{
    public string Id { get; }
    public string Name { get; }
    public SourceReference Source { get; }
    public IReadOnlyList<AbilityType> RestrictedBoostOptions { get; }
    public int FreeBoostCount { get; }
    public IReadOnlyList<BackgroundGrantDescriptor> Grants { get; }

    public Background(
        string id,
        string name,
        SourceReference source,
        IReadOnlyList<AbilityType> restrictedBoostOptions,
        int freeBoostCount,
        IReadOnlyList<BackgroundGrantDescriptor> grants )
    {
        if ( String.IsNullOrWhiteSpace( id ) )
        {
            throw new ArgumentException( "Background id cannot be empty.", nameof( id ) );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Background name cannot be empty.", nameof( name ) );
        }

        ArgumentNullException.ThrowIfNull( source );
        ArgumentNullException.ThrowIfNull( restrictedBoostOptions );
        ArgumentNullException.ThrowIfNull( grants );

        if ( restrictedBoostOptions.Count < 1 )
        {
            throw new ArgumentException( "Background must define restricted boost options.", nameof( restrictedBoostOptions ) );
        }

        if ( restrictedBoostOptions.Distinct().Count() != restrictedBoostOptions.Count )
        {
            throw new ArgumentException( "Background restricted boost options must be unique.", nameof( restrictedBoostOptions ) );
        }

        if ( freeBoostCount != 1 )
        {
            throw new ArgumentOutOfRangeException( nameof( freeBoostCount ), "The current background baseline requires exactly one free boost." );
        }

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        RestrictedBoostOptions = restrictedBoostOptions;
        FreeBoostCount = freeBoostCount;
        Grants = grants;
    }
}
