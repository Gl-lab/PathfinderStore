using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Restocking;

namespace Pathfinder.Commerce.Domain.Tests.Restocking;

public sealed class RestockPolicyTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 27, 8, 0, 0, TimeSpan.Zero );

    [Fact]
    public void RevisePreservesPreviousRevision()
    {
        RestockPolicy policy = RestockPolicy.Create(
            7,
            11,
            "General",
            8,
            DefaultConstraints(),
            DefaultWeights(),
            13,
            _now );

        policy.Revise(
            1,
            12,
            DefaultConstraints(),
            new RestockSelectionWeights( 4, 2, 1 ),
            14,
            _now.AddHours( 1 ) );

        Assert.Equal( 2, policy.CurrentVersion );
        Assert.Collection(
            policy.Revisions.OrderBy( revision => revision.Version ),
            revision => Assert.Equal( 8, revision.TargetOfferCount ),
            revision => Assert.Equal( 12, revision.TargetOfferCount ) );
        Assert.Equal( 1, policy.Revisions
            .Single( revision => revision.Version == 1 )
            .ConsumableWeight );
        Assert.Equal( 4, policy.CurrentRevision.ConsumableWeight );
    }

    [Fact]
    public void ReviseRejectsStaleExpectedVersion()
    {
        RestockPolicy policy = RestockPolicy.Create(
            7,
            11,
            "General",
            8,
            DefaultConstraints(),
            DefaultWeights(),
            13,
            _now );

        CommerceException exception = Assert.Throws<CommerceException>(
            () => policy.Revise(
                0,
                12,
                DefaultConstraints(),
                DefaultWeights(),
                14,
                _now.AddHours( 1 ) ) );

        Assert.Contains( "version conflict", exception.Message );
        Assert.Single( policy.Revisions );
    }

    [Fact]
    public void RevisionRejectsCandidateOutsidePolicy()
    {
        RestockPolicyConstraints constraints = new RestockPolicyConstraints(
            2,
            5,
            1000,
            RestockItemRarity.Common,
            RestockItemAccess.Global,
            RestockItemCategory.Weapon );
        RestockPolicy policy = RestockPolicy.Create(
            7,
            11,
            "Weapons",
            4,
            constraints,
            DefaultWeights(),
            13,
            _now );

        bool allowed = policy.CurrentRevision.Allows(
            new RestockCandidate(
                17,
                4,
                500,
                RestockItemRarity.Rare,
                RestockItemAccess.Global,
                RestockItemCategory.Weapon,
                RestockItemKind.Permanent ),
            1000 );

        Assert.False( allowed );
    }

    [Fact]
    public void RevisionAppliesLevelBudgetAccessAndCategoryTogether()
    {
        RestockPolicyConstraints constraints = new RestockPolicyConstraints(
            2,
            5,
            1000,
            RestockItemRarity.Common,
            RestockItemAccess.Global,
            RestockItemCategory.Weapon );
        RestockPolicy policy = RestockPolicy.Create(
            7,
            11,
            "Weapons",
            4,
            constraints,
            DefaultWeights(),
            13,
            _now );
        RestockCandidate candidate = new RestockCandidate(
            17,
            4,
            500,
            RestockItemRarity.Common,
            RestockItemAccess.Global,
            RestockItemCategory.Weapon,
            RestockItemKind.Permanent );

        Assert.True( policy.CurrentRevision.Allows( candidate, 500 ) );
        Assert.False( policy.CurrentRevision.Allows( candidate with { Level = 6 }, 500 ) );
        Assert.False( policy.CurrentRevision.Allows( candidate, 499 ) );
        Assert.False( policy.CurrentRevision.Allows(
            candidate with { Access = RestockItemAccess.Campaign },
            500 ) );
        Assert.False( policy.CurrentRevision.Allows(
            candidate with { Category = RestockItemCategory.Armor },
            500 ) );
    }

    [Fact]
    public void RevisionReturnsWeightForCandidateKind()
    {
        RestockPolicy policy = RestockPolicy.Create(
            7,
            11,
            "Weighted",
            4,
            DefaultConstraints(),
            new RestockSelectionWeights( 5, 2, 0 ),
            13,
            _now );
        RestockCandidate candidate = new RestockCandidate(
            17,
            4,
            500,
            RestockItemRarity.Common,
            RestockItemAccess.Global,
            RestockItemCategory.Consumable,
            RestockItemKind.Consumable );

        Assert.Equal( 5, policy.CurrentRevision.GetSelectionWeight( candidate ) );
        Assert.Equal( 0, policy.CurrentRevision.GetSelectionWeight(
            candidate with { Kind = RestockItemKind.Unique } ) );
    }

    [Fact]
    public void WeightsRejectDistributionWithoutSelectableKind()
    {
        Assert.Throws<CommerceException>(
            () => new RestockSelectionWeights( 0, 0, 0 ) );
    }

    private static RestockPolicyConstraints DefaultConstraints() =>
        new RestockPolicyConstraints(
            0,
            20,
            10000,
            RestockItemRarity.All,
            RestockItemAccess.All,
            RestockItemCategory.All );

    private static RestockSelectionWeights DefaultWeights() =>
        new RestockSelectionWeights( 1, 1, 0 );
}
