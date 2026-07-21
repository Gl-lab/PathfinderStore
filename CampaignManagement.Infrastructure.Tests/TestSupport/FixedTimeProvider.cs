namespace Pathfinder.CampaignManagement.Infrastructure.Tests.TestSupport;

internal sealed class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _utcNow;

    public FixedTimeProvider( DateTimeOffset utcNow )
    {
        _utcNow = utcNow;
    }

    public override DateTimeOffset GetUtcNow() => _utcNow;
}
