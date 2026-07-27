using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Restocking;

public sealed class RestockPolicyRevision : Entity
{
    private RestockPolicyRevision()
    {
    }

    public int RestockPolicyId { get; private set; }
    public int Version { get; private set; }
    public int TargetOfferCount { get; private set; }
    public int CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    internal static RestockPolicyRevision Create(
        int version,
        int targetOfferCount,
        int createdByUserId,
        DateTimeOffset createdAtUtc )
    {
        if ( version <= 0 )
        {
            throw new CommerceException( "Restock policy revision version must be greater than zero." );
        }

        if ( targetOfferCount <= 0 )
        {
            throw new CommerceException( "Restock target offer count must be greater than zero." );
        }

        if ( createdByUserId <= 0 )
        {
            throw new CommerceException( "Restock policy revision author must be greater than zero." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new CommerceException( "Restock policy revision timestamp must use UTC." );
        }

        return new RestockPolicyRevision
        {
            Version = version,
            TargetOfferCount = targetOfferCount,
            CreatedByUserId = createdByUserId,
            CreatedAtUtc = createdAtUtc,
        };
    }
}
