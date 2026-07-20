using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Feats;

public static class FeatCatalogResolver
{
    public static IReadOnlyCollection<FeatDefinition> ResolveOptions(
        IReadOnlyCollection<FeatDefinition> feats,
        FeatCategory category,
        int level,
        string? requiredTrait = null )
    {
        ArgumentNullException.ThrowIfNull( feats );

        if ( level < 1 )
        {
            throw new ArgumentOutOfRangeException( nameof( level ), "Feat level must be positive." );
        }

        return feats
            .Where( feat => feat.Category == category )
            .Where( feat => feat.Level == level )
            .Where( feat => feat.Rarity == FeatRarity.Common )
            .Where( feat => String.IsNullOrWhiteSpace( requiredTrait ) ||
                            feat.Traits.Contains( requiredTrait.Trim(), StringComparer.OrdinalIgnoreCase ) )
            .OrderBy( feat => feat.Name, StringComparer.Ordinal )
            .ToArray();
    }
}
