using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace CharacterManagement.Domain.Tests;

public sealed class FeatCatalogResolverTests
{
    [Fact]
    public void ResolveOptions_ReturnsOnlyCommonMatchingOrderedFeats()
    {
        IReadOnlyCollection<FeatDefinition> catalog =
        [
            Feat( "feat.zeta", "Zeta", FeatCategory.Class, FeatRarity.Common, "Fighter" ),
            Feat( "feat.alpha", "Alpha", FeatCategory.Class, FeatRarity.Common, "Fighter" ),
            Feat( "feat.uncommon", "Uncommon", FeatCategory.Class, FeatRarity.Uncommon, "Fighter" ),
            Feat( "feat.skill", "Skill", FeatCategory.Skill, FeatRarity.Common, "Fighter" ),
            Feat( "feat.rogue", "Rogue", FeatCategory.Class, FeatRarity.Common, "Rogue" ),
        ];

        IReadOnlyCollection<FeatDefinition> result = FeatCatalogResolver.ResolveOptions(
            catalog,
            FeatCategory.Class,
            1,
            "fighter" );

        Assert.Equal( [ "feat.alpha", "feat.zeta" ], result.Select( feat => feat.Id ) );
    }

    [Fact]
    public void ResolveOptions_RejectsNonPositiveLevel()
    {
        Assert.Throws<ArgumentOutOfRangeException>( () => FeatCatalogResolver.ResolveOptions(
            [],
            FeatCategory.Skill,
            0 ) );
    }

    private static FeatDefinition Feat(
        string id,
        string name,
        FeatCategory category,
        FeatRarity rarity,
        string trait )
    {
        return new FeatDefinition(
            id,
            name,
            category,
            1,
            [ trait ],
            rarity,
            [],
            "Deferred effect.",
            [ FeatDependencyType.RuleEngine ],
            new SourceReference( "Player Core", 1 ) );
    }
}
