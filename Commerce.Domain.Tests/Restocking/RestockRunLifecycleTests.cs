using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Restocking;

namespace Pathfinder.Commerce.Domain.Tests.Restocking;

public sealed class RestockRunLifecycleTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 27, 8, 0, 0, TimeSpan.Zero );

    [Fact]
    public void ConfirmRequiresEveryLineToBePublished()
    {
        RestockRun run = CreateRun();

        CommerceException exception = Assert.Throws<CommerceException>(
            () => run.Confirm( 11, _now.AddMinutes( 1 ) ) );

        Assert.Contains( "Every restock run line", exception.Message );
        Assert.Equal( RestockRunStatus.Preview, run.Status );
    }

    [Fact]
    public void RejectedPreviewCannotBeConfirmed()
    {
        RestockRun run = CreateRun();

        run.Reject( 11, _now.AddMinutes( 1 ) );

        Assert.Throws<CommerceException>(
            () => run.Confirm( 11, _now.AddMinutes( 2 ) ) );
        Assert.Equal( RestockRunStatus.Rejected, run.Status );
    }

    [Fact]
    public void UniqueLineRequiresExactlyOneInstanceIdentity()
    {
        RestockRun run = CreateRun();
        RestockRunLine line = Assert.Single( run.Lines );

        Assert.Throws<CommerceException>(
            () => line.Publish( Guid.NewGuid(), null ) );
        Guid instanceKey = Guid.NewGuid();
        line.Publish( Guid.NewGuid(), instanceKey );
        run.Confirm( 11, _now.AddMinutes( 1 ) );

        Assert.Equal( instanceKey, line.PublishedItemInstanceKey );
        Assert.Equal( RestockRunStatus.Confirmed, run.Status );
    }

    private static RestockRun CreateRun() => RestockRun.CreatePreview(
        7,
        11,
        13,
        1,
        42,
        11,
        _now,
        [
            new RestockCandidate(
                17,
                4,
                500,
                RestockItemRarity.Unique,
                RestockItemAccess.Campaign,
                RestockItemCategory.OtherEquipment,
                RestockItemKind.Unique ),
        ] );
}
