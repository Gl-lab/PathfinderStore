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
        RestockPolicy policy = RestockPolicy.Create( 7, 11, "General", 8, 13, _now );

        policy.Revise( 1, 12, 14, _now.AddHours( 1 ) );

        Assert.Equal( 2, policy.CurrentVersion );
        Assert.Collection(
            policy.Revisions.OrderBy( revision => revision.Version ),
            revision => Assert.Equal( 8, revision.TargetOfferCount ),
            revision => Assert.Equal( 12, revision.TargetOfferCount ) );
    }

    [Fact]
    public void ReviseRejectsStaleExpectedVersion()
    {
        RestockPolicy policy = RestockPolicy.Create( 7, 11, "General", 8, 13, _now );

        CommerceException exception = Assert.Throws<CommerceException>(
            () => policy.Revise( 0, 12, 14, _now.AddHours( 1 ) ) );

        Assert.Contains( "version conflict", exception.Message );
        Assert.Single( policy.Revisions );
    }
}
