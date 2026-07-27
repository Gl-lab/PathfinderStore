using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Restocking;

public sealed class RestockRun : Entity, IAggregateRoot
{
    private readonly List<RestockRunLine> _lines = [];

    private RestockRun()
    {
    }

    public Guid RunKey { get; private set; }
    public int CampaignId { get; private set; }
    public int ShopId { get; private set; }
    public int RestockPolicyId { get; private set; }
    public int PolicyVersion { get; private set; }
    public long Seed { get; private set; }
    public RestockRunStatus Status { get; private set; }
    public int CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public int? CompletedByUserId { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public IReadOnlyCollection<RestockRunLine> Lines { get => _lines; }
    public long TotalPriceCopper { get => _lines.Sum(
        line => checked( line.UnitPriceCopper * line.Quantity ) ); }

    public static RestockRun CreatePreview(
        int campaignId,
        int shopId,
        int restockPolicyId,
        int policyVersion,
        long seed,
        int createdByUserId,
        DateTimeOffset createdAtUtc,
        IReadOnlyCollection<RestockCandidate> candidates )
    {
        if ( campaignId <= 0 ||
             shopId <= 0 ||
             restockPolicyId <= 0 ||
             policyVersion <= 0 ||
             createdByUserId <= 0 )
        {
            throw new CommerceException( "Restock run identity is invalid." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new CommerceException( "Restock run timestamp must use UTC." );
        }

        ArgumentNullException.ThrowIfNull( candidates );
        RestockRun run = new RestockRun
        {
            RunKey = Guid.NewGuid(),
            CampaignId = campaignId,
            ShopId = shopId,
            RestockPolicyId = restockPolicyId,
            PolicyVersion = policyVersion,
            Seed = seed,
            Status = RestockRunStatus.Preview,
            CreatedByUserId = createdByUserId,
            CreatedAtUtc = createdAtUtc,
        };
        int sequence = 1;
        foreach ( RestockCandidate candidate in candidates )
        {
            run._lines.Add( RestockRunLine.Create( sequence, candidate ) );
            sequence++;
        }

        return run;
    }

    public void Confirm( int actingUserId, DateTimeOffset confirmedAtUtc )
    {
        if ( Status == RestockRunStatus.Confirmed )
        {
            return;
        }

        EnsureCanComplete( actingUserId, confirmedAtUtc );
        if ( _lines.Any( line => line.PublishedOfferKey is null ) )
        {
            throw new CommerceException(
                "Every restock run line must be published before confirmation." );
        }

        Status = RestockRunStatus.Confirmed;
        CompletedByUserId = actingUserId;
        CompletedAtUtc = confirmedAtUtc;
    }

    public void Reject( int actingUserId, DateTimeOffset rejectedAtUtc )
    {
        if ( Status == RestockRunStatus.Rejected )
        {
            return;
        }

        EnsureCanComplete( actingUserId, rejectedAtUtc );
        Status = RestockRunStatus.Rejected;
        CompletedByUserId = actingUserId;
        CompletedAtUtc = rejectedAtUtc;
    }

    private void EnsureCanComplete( int actingUserId, DateTimeOffset completedAtUtc )
    {
        if ( Status != RestockRunStatus.Preview )
        {
            throw new CommerceException( "Only a restock preview can be completed." );
        }

        if ( actingUserId <= 0 )
        {
            throw new CommerceException( "Restock run completer must be greater than zero." );
        }

        if ( completedAtUtc.Offset != TimeSpan.Zero || completedAtUtc < CreatedAtUtc )
        {
            throw new CommerceException(
                "Restock run completion timestamp must use UTC and follow creation." );
        }
    }
}
