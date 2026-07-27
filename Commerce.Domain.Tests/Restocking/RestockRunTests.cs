using Pathfinder.Commerce.Domain.Restocking;

namespace Pathfinder.Commerce.Domain.Tests.Restocking;

public sealed class RestockRunTests
{
    [Fact]
    public void SelectorReproducesSameResultForSeedAndRevision()
    {
        RestockPolicy policy = CreatePolicy();
        IReadOnlyCollection<RestockCandidate> candidates = CreateCandidates();
        DeterministicRestockSelector selector = new DeterministicRestockSelector();

        IReadOnlyCollection<int> first = selector
            .Select( policy.CurrentRevision, 12345, candidates )
            .Select( candidate => candidate.ItemConfigurationId )
            .ToArray();
        IReadOnlyCollection<int> second = selector
            .Select( policy.CurrentRevision, 12345, candidates.Reverse().ToArray() )
            .Select( candidate => candidate.ItemConfigurationId )
            .ToArray();

        Assert.Equal( first, second );
        Assert.Equal( 3, first.Count );
    }

    [Fact]
    public void SelectorRespectsBudgetAndDoesNotSelectConfigurationTwice()
    {
        RestockPolicy policy = CreatePolicy();
        DeterministicRestockSelector selector = new DeterministicRestockSelector();

        IReadOnlyCollection<RestockCandidate> selected = selector.Select(
            policy.CurrentRevision,
            42,
            CreateCandidates() );

        Assert.True( selected.Sum( candidate => candidate.UnitPriceCopper ) <= 900 );
        Assert.Equal(
            selected.Count,
            selected.Select( candidate => candidate.ItemConfigurationId ).Distinct().Count() );
    }

    private static RestockPolicy CreatePolicy() => RestockPolicy.Create(
        7,
        11,
        "Generator",
        3,
        new RestockPolicyConstraints(
            0,
            20,
            900,
            RestockItemRarity.All,
            RestockItemAccess.All,
            RestockItemCategory.All ),
        new RestockSelectionWeights( 3, 2, 1 ),
        13,
        new DateTimeOffset( 2026, 7, 27, 8, 0, 0, TimeSpan.Zero ) );

    private static IReadOnlyCollection<RestockCandidate> CreateCandidates() =>
    [
        new RestockCandidate(
            1,
            1,
            200,
            RestockItemRarity.Common,
            RestockItemAccess.Global,
            RestockItemCategory.Consumable,
            RestockItemKind.Consumable ),
        new RestockCandidate(
            2,
            2,
            300,
            RestockItemRarity.Common,
            RestockItemAccess.Global,
            RestockItemCategory.Weapon,
            RestockItemKind.Permanent ),
        new RestockCandidate(
            3,
            3,
            400,
            RestockItemRarity.Common,
            RestockItemAccess.Global,
            RestockItemCategory.Armor,
            RestockItemKind.Permanent ),
        new RestockCandidate(
            4,
            4,
            500,
            RestockItemRarity.Unique,
            RestockItemAccess.Campaign,
            RestockItemCategory.OtherEquipment,
            RestockItemKind.Unique ),
    ];
}
