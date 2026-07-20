namespace Pathfinder.CharacterManagement.Domain.Rules.Feats;

public sealed record FeatChoice
{
    public string SourceId { get; init; }
    public string FeatId { get; init; }

    public FeatChoice( string sourceId, string featId )
    {
        if ( String.IsNullOrWhiteSpace( sourceId ) )
        {
            throw new ArgumentException( "Feat choice source id cannot be empty.", nameof( sourceId ) );
        }

        if ( String.IsNullOrWhiteSpace( featId ) )
        {
            throw new ArgumentException( "Feat choice feat id cannot be empty.", nameof( featId ) );
        }

        SourceId = sourceId.Trim();
        FeatId = featId.Trim();
    }
}
