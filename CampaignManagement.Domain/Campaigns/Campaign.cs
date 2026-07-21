using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CampaignManagement.Domain.Campaigns;

public sealed class Campaign : Entity, IAggregateRoot
{
    public const int NameMaxLength = 200;

    private readonly List<CampaignMembership> _memberships = [];

    private Campaign()
    {
    }

    public string Name { get; private set; } = String.Empty;
    public CampaignStatus Status { get; private set; }
    public int CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ArchivedAtUtc { get; private set; }
    public IReadOnlyList<CampaignMembership> Memberships { get => _memberships; }

    public static Campaign Create( string name, int creatorUserId, DateTimeOffset createdAtUtc )
    {
        string normalizedName = NormalizeName( name );
        EnsurePositiveUserId( creatorUserId );
        EnsureUtc( createdAtUtc, "Creation timestamp" );

        Campaign campaign = new Campaign
        {
            Name = normalizedName,
            Status = CampaignStatus.Active,
            CreatedByUserId = creatorUserId,
            CreatedAtUtc = createdAtUtc,
        };
        campaign._memberships.Add( CampaignMembership.CreateGameMaster( creatorUserId, createdAtUtc ) );
        campaign.EnsureInvariants();
        return campaign;
    }

    public void Archive( int actingUserId, DateTimeOffset archivedAtUtc )
    {
        EnsureUtc( archivedAtUtc, "Archive timestamp" );

        if ( !HasActiveRole( actingUserId, CampaignMembershipRole.GameMaster ) )
        {
            throw new CampaignManagementException( "Only an active campaign Game Master can archive the campaign." );
        }

        if ( Status == CampaignStatus.Archived )
        {
            return;
        }

        if ( archivedAtUtc < CreatedAtUtc )
        {
            throw new CampaignManagementException( "Archive timestamp cannot precede campaign creation." );
        }

        Status = CampaignStatus.Archived;
        ArchivedAtUtc = archivedAtUtc;
        EnsureInvariants();
    }

    public bool HasActiveRole( int userId, CampaignMembershipRole role ) =>
        _memberships.Any( membership =>
            ( membership.UserId == userId ) &&
            ( membership.Role == role ) &&
            ( membership.Status == CampaignMembershipStatus.Active ) );

    private static string NormalizeName( string name )
    {
        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new CampaignManagementException( "Campaign name cannot be empty." );
        }

        string normalizedName = name.Trim();
        if ( normalizedName.Length > NameMaxLength )
        {
            throw new CampaignManagementException( $"Campaign name cannot exceed {NameMaxLength} characters." );
        }

        return normalizedName;
    }

    private static void EnsurePositiveUserId( int userId )
    {
        if ( userId <= 0 )
        {
            throw new CampaignManagementException( "Creator user id must be greater than zero." );
        }
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new CampaignManagementException( $"{fieldName} must use UTC." );
        }
    }

    private void EnsureInvariants()
    {
        if ( !_memberships.Any( membership =>
                 ( membership.UserId == CreatedByUserId ) &&
                 ( membership.Role == CampaignMembershipRole.GameMaster ) ) )
        {
            throw new CampaignManagementException( "Campaign creator must have the Game Master role." );
        }

        if ( ( Status == CampaignStatus.Active ) != ( ArchivedAtUtc is null ) )
        {
            throw new CampaignManagementException( "Campaign status and archive timestamp are inconsistent." );
        }
    }
}
