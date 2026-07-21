using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CampaignManagement.Domain.Campaigns;

public sealed class Campaign : Entity, IAggregateRoot
{
    public const int NameMaxLength = 200;

    private readonly List<CampaignMembership> _memberships = [];
    private readonly List<CampaignInvitation> _invitations = [];

    private Campaign()
    {
    }

    public string Name { get; private set; } = String.Empty;
    public CampaignStatus Status { get; private set; }
    public int CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ArchivedAtUtc { get; private set; }
    public IReadOnlyList<CampaignMembership> Memberships { get => _memberships; }
    public IReadOnlyList<CampaignInvitation> Invitations { get => _invitations; }

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

    public CampaignInvitation Invite(
        int actingUserId,
        int invitedUserId,
        DateTimeOffset createdAtUtc )
    {
        EnsureActive();
        EnsureGameMaster( actingUserId );
        if ( HasActiveMembership( invitedUserId ) )
        {
            throw new CampaignManagementException( "User is already an active campaign member." );
        }

        if ( _invitations.Any( invitation =>
                 ( invitation.InvitedUserId == invitedUserId ) &&
                 ( invitation.Status == CampaignInvitationStatus.Pending ) ) )
        {
            throw new CampaignManagementException( "User already has a pending campaign invitation." );
        }

        CampaignInvitation invitation = CampaignInvitation.Create(
            invitedUserId,
            actingUserId,
            createdAtUtc );
        _invitations.Add( invitation );
        return invitation;
    }

    public void AcceptInvitation(
        int invitationId,
        int actingUserId,
        DateTimeOffset respondedAtUtc )
    {
        EnsureActive();
        CampaignInvitation invitation = GetInvitation( invitationId );
        if ( HasActiveMembership( actingUserId ) )
        {
            throw new CampaignManagementException( "User is already an active campaign member." );
        }

        invitation.Accept( actingUserId, respondedAtUtc );
        AddOrReactivateRole( actingUserId, CampaignMembershipRole.Player, respondedAtUtc );
        EnsureInvariants();
    }

    public void DeclineInvitation(
        int invitationId,
        int actingUserId,
        DateTimeOffset respondedAtUtc )
    {
        EnsureActive();
        GetInvitation( invitationId )
            .Decline( actingUserId, respondedAtUtc );
    }

    public void AssignRole(
        int actingUserId,
        int memberUserId,
        CampaignMembershipRole role,
        DateTimeOffset assignedAtUtc )
    {
        EnsureActive();
        EnsureGameMaster( actingUserId );
        if ( !HasActiveMembership( memberUserId ) )
        {
            throw new CampaignManagementException( "Campaign member was not found." );
        }

        AddOrReactivateRole( memberUserId, role, assignedAtUtc );
        EnsureInvariants();
    }

    public void RevokeRole(
        int actingUserId,
        int memberUserId,
        CampaignMembershipRole role )
    {
        EnsureActive();
        EnsureGameMaster( actingUserId );
        CampaignMembership? membership = _memberships.SingleOrDefault( item =>
            ( item.UserId == memberUserId ) &&
            ( item.Role == role ) &&
            ( item.Status == CampaignMembershipStatus.Active ) );
        if ( membership is null )
        {
            throw new CampaignManagementException( "Active campaign role was not found." );
        }

        if ( ( role == CampaignMembershipRole.GameMaster ) &&
             ( CountActiveGameMasters() == 1 ) )
        {
            throw new CampaignManagementException( "The last active Game Master role cannot be removed." );
        }

        membership.Leave();
        EnsureInvariants();
    }

    public void Leave( int actingUserId )
    {
        EnsureActive();
        IReadOnlyCollection<CampaignMembership> activeMemberships = _memberships
            .Where( membership =>
                ( membership.UserId == actingUserId ) &&
                ( membership.Status == CampaignMembershipStatus.Active ) )
            .ToArray();
        if ( activeMemberships.Count == 0 )
        {
            throw new CampaignManagementException( "Active campaign membership was not found." );
        }

        bool isGameMaster = activeMemberships.Any( membership =>
            membership.Role == CampaignMembershipRole.GameMaster );
        if ( isGameMaster && ( CountActiveGameMasters() == 1 ) )
        {
            throw new CampaignManagementException( "The last active Game Master cannot leave the campaign." );
        }

        foreach ( CampaignMembership membership in activeMemberships )
        {
            membership.Leave();
        }

        EnsureInvariants();
    }

    private bool HasActiveMembership( int userId ) => _memberships.Any( membership =>
        ( membership.UserId == userId ) &&
        ( membership.Status == CampaignMembershipStatus.Active ) );

    private CampaignInvitation GetInvitation( int invitationId )
    {
        CampaignInvitation? invitation = _invitations.SingleOrDefault( item => item.Id == invitationId );
        return invitation ?? throw new CampaignManagementException( "Campaign invitation was not found." );
    }

    private void AddOrReactivateRole(
        int userId,
        CampaignMembershipRole role,
        DateTimeOffset joinedAtUtc )
    {
        CampaignMembership? existingMembership = _memberships.SingleOrDefault( membership =>
            ( membership.UserId == userId ) &&
            ( membership.Role == role ) );
        if ( existingMembership is null )
        {
            _memberships.Add( CampaignMembership.Create( userId, role, joinedAtUtc ) );
            return;
        }

        if ( existingMembership.Status == CampaignMembershipStatus.Left )
        {
            existingMembership.Activate( joinedAtUtc );
        }
    }

    private void EnsureActive()
    {
        if ( Status != CampaignStatus.Active )
        {
            throw new CampaignManagementException( "Archived campaign membership cannot be changed." );
        }
    }

    private void EnsureGameMaster( int userId )
    {
        if ( !HasActiveRole( userId, CampaignMembershipRole.GameMaster ) )
        {
            throw new CampaignManagementException( "Only an active campaign Game Master can perform this operation." );
        }
    }

    private int CountActiveGameMasters() => _memberships.Count( membership =>
        ( membership.Role == CampaignMembershipRole.GameMaster ) &&
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

        if ( CountActiveGameMasters() == 0 )
        {
            throw new CampaignManagementException( "Campaign must have an active Game Master." );
        }

        if ( ( Status == CampaignStatus.Active ) != ( ArchivedAtUtc is null ) )
        {
            throw new CampaignManagementException( "Campaign status and archive timestamp are inconsistent." );
        }
    }
}
