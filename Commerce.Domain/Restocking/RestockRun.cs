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
}
