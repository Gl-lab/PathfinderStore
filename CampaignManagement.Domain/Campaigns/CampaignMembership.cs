using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CampaignManagement.Domain.Campaigns;

public sealed class CampaignMembership : Entity
{
    private CampaignMembership()
    {
    }

    public int CampaignId { get; private set; }
    public int UserId { get; private set; }
    public CampaignMembershipRole Role { get; private set; }
    public CampaignMembershipStatus Status { get; private set; }
    public DateTimeOffset JoinedAtUtc { get; private set; }

    internal static CampaignMembership CreateGameMaster( int userId, DateTimeOffset joinedAtUtc )
    {
        return Create( userId, CampaignMembershipRole.GameMaster, joinedAtUtc );
    }

    internal static CampaignMembership Create(
        int userId,
        CampaignMembershipRole role,
        DateTimeOffset joinedAtUtc )
    {
        if ( userId <= 0 )
        {
            throw new CampaignManagementException( "UserId must be greater than zero." );
        }

        EnsureUtc( joinedAtUtc, "Membership timestamp" );
        if ( !Enum.IsDefined( role ) )
        {
            throw new CampaignManagementException( $"Unknown campaign membership role '{role}'." );
        }

        return new CampaignMembership
        {
            UserId = userId,
            Role = role,
            Status = CampaignMembershipStatus.Active,
            JoinedAtUtc = joinedAtUtc,
        };
    }

    internal void Leave()
    {
        Status = CampaignMembershipStatus.Left;
    }

    internal void Activate( DateTimeOffset joinedAtUtc )
    {
        EnsureUtc( joinedAtUtc, "Membership timestamp" );
        Status = CampaignMembershipStatus.Active;
        JoinedAtUtc = joinedAtUtc;
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new CampaignManagementException( $"{fieldName} must use UTC." );
        }
    }
}
