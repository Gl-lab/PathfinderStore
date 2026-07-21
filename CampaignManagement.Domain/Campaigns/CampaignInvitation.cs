using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CampaignManagement.Domain.Campaigns;

public sealed class CampaignInvitation : Entity
{
    private CampaignInvitation()
    {
    }

    public int CampaignId { get; private set; }
    public int InvitedUserId { get; private set; }
    public int InvitedByUserId { get; private set; }
    public CampaignInvitationStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? RespondedAtUtc { get; private set; }

    internal static CampaignInvitation Create(
        int invitedUserId,
        int invitedByUserId,
        DateTimeOffset createdAtUtc )
    {
        EnsurePositiveUserId( invitedUserId, "Invited user id" );
        EnsurePositiveUserId( invitedByUserId, "Inviting user id" );
        EnsureUtc( createdAtUtc, "Invitation timestamp" );

        return new CampaignInvitation
        {
            InvitedUserId = invitedUserId,
            InvitedByUserId = invitedByUserId,
            Status = CampaignInvitationStatus.Pending,
            CreatedAtUtc = createdAtUtc,
        };
    }

    internal void Accept( int actingUserId, DateTimeOffset respondedAtUtc )
    {
        EnsureCanRespond( actingUserId, respondedAtUtc );
        Status = CampaignInvitationStatus.Accepted;
        RespondedAtUtc = respondedAtUtc;
    }

    internal void Decline( int actingUserId, DateTimeOffset respondedAtUtc )
    {
        EnsureCanRespond( actingUserId, respondedAtUtc );
        Status = CampaignInvitationStatus.Declined;
        RespondedAtUtc = respondedAtUtc;
    }

    internal void Cancel( DateTimeOffset cancelledAtUtc )
    {
        EnsureUtc( cancelledAtUtc, "Invitation cancellation timestamp" );
        if ( Status != CampaignInvitationStatus.Pending )
        {
            return;
        }

        if ( cancelledAtUtc < CreatedAtUtc )
        {
            throw new CampaignManagementException(
                "Invitation cancellation cannot precede its creation." );
        }

        Status = CampaignInvitationStatus.Cancelled;
        RespondedAtUtc = cancelledAtUtc;
    }

    private void EnsureCanRespond( int actingUserId, DateTimeOffset respondedAtUtc )
    {
        EnsureUtc( respondedAtUtc, "Invitation response timestamp" );
        if ( actingUserId != InvitedUserId )
        {
            throw new CampaignManagementException( "Only the invited user can respond to an invitation." );
        }

        if ( Status != CampaignInvitationStatus.Pending )
        {
            throw new CampaignManagementException( "Only a pending invitation can be answered." );
        }

        if ( respondedAtUtc < CreatedAtUtc )
        {
            throw new CampaignManagementException( "Invitation response cannot precede its creation." );
        }
    }

    private static void EnsurePositiveUserId( int userId, string fieldName )
    {
        if ( userId <= 0 )
        {
            throw new CampaignManagementException( $"{fieldName} must be greater than zero." );
        }
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new CampaignManagementException( $"{fieldName} must use UTC." );
        }
    }
}
