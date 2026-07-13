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

        ValidateGrants( grants );

        Id = id.Trim();
        Name = name.Trim();
        Source = source;
        RestrictedBoostOptions = restrictedBoostOptions;
        FreeBoostCount = freeBoostCount;
        Grants = grants;
    }

    private static void ValidateGrants( IReadOnlyList<BackgroundGrantDescriptor> grants )
    {
        if ( grants.Count == 0 )
        {
            return;
        }

        if ( grants.Select( grant => grant.Id ).Distinct( StringComparer.Ordinal ).Count() != grants.Count )
        {
            throw new ArgumentException( "Background grant ids must be unique.", nameof( grants ) );
        }

        if ( grants.Count( grant => grant.Kind == BackgroundGrantKind.SkillTraining ) != 1 )
        {
            throw new ArgumentException( "Background must define exactly one skill training grant.", nameof( grants ) );
        }

        if ( grants.Count( grant => grant.Kind == BackgroundGrantKind.LoreTraining ) != 1 )
        {
            throw new ArgumentException( "Background must define exactly one Lore training grant.", nameof( grants ) );
        }

        foreach ( BackgroundGrantDescriptor grant in grants )
        {
            if ( String.IsNullOrWhiteSpace( grant.Id ) )
            {
                throw new ArgumentException( "Background grant id cannot be empty.", nameof( grants ) );
            }

            if ( grant.RequiresChoice )
            {
                ValidateChoiceGrant( grant, grants );
            }
            else if ( String.IsNullOrWhiteSpace( grant.TargetId ) ||
                      grant.AllowsCustomLore ||
                      ( grant.Options.Count > 0 ) )
            {
                throw new ArgumentException( "Fixed background grant must define only its target id.", nameof( grants ) );
            }
        }
    }

    private static void ValidateChoiceGrant(
        BackgroundGrantDescriptor grant,
        IReadOnlyList<BackgroundGrantDescriptor> grants )
    {
        if ( !String.IsNullOrWhiteSpace( grant.TargetId ) )
        {
            throw new ArgumentException( "Choice background grant cannot define a fixed target id.", nameof( grants ) );
        }

        if ( grant.AllowsCustomLore )
        {
            if ( ( grant.Kind != BackgroundGrantKind.LoreTraining ) || ( grant.Options.Count > 0 ) )
            {
                throw new ArgumentException( "Open choice is supported only for Lore without fixed options.", nameof( grants ) );
            }

            return;
        }

        if ( grant.Options.Count == 0 )
        {
            throw new ArgumentException( "Choice background grant must define options.", nameof( grants ) );
        }

        if ( grant.Options.Select( option => option.Id ).Distinct( StringComparer.Ordinal ).Count() != grant.Options.Count )
        {
            throw new ArgumentException( "Background grant options must be unique.", nameof( grants ) );
        }
    }
}
