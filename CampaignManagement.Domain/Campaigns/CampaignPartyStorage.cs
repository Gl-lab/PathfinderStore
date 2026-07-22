using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CampaignManagement.Domain.Campaigns;

public sealed class CampaignPartyStorage : Entity
{
    private CampaignPartyStorage()
    {
    }

    public int CampaignPartyId { get; private set; }
    public CampaignPartyStorageAccessPolicy AccessPolicy { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    internal static CampaignPartyStorage Create( DateTimeOffset createdAtUtc )
    {
        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new CampaignManagementException( "Party storage creation timestamp must use UTC." );
        }

        return new CampaignPartyStorage
        {
            AccessPolicy = CampaignPartyStorageAccessPolicy.Unconfigured,
            CreatedAtUtc = createdAtUtc,
        };
    }

    internal void SetAccessPolicy( CampaignPartyStorageAccessPolicy accessPolicy )
    {
        if ( !Enum.IsDefined( accessPolicy ) ||
             ( accessPolicy == CampaignPartyStorageAccessPolicy.Unconfigured ) )
        {
            throw new CampaignManagementException( "Party storage access policy is invalid." );
        }

        AccessPolicy = accessPolicy;
    }
}
