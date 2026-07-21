using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Domain.Exceptions;

namespace Pathfinder.CampaignManagement.Domain.Tests;

public sealed class CampaignTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 21, 10, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateAssignsActiveGameMasterMembershipToCreator()
    {
        Campaign campaign = Campaign.Create( "  Abomination Vaults  ", 42, _createdAtUtc );

        Assert.Equal( "Abomination Vaults", campaign.Name );
        Assert.Equal( CampaignStatus.Active, campaign.Status );
        Assert.Equal( 42, campaign.CreatedByUserId );
        CampaignMembership membership = Assert.Single( campaign.Memberships );
        Assert.Equal( 42, membership.UserId );
        Assert.Equal( CampaignMembershipRole.GameMaster, membership.Role );
        Assert.Equal( CampaignMembershipStatus.Active, membership.Status );
        Assert.True( campaign.HasActiveRole( 42, CampaignMembershipRole.GameMaster ) );
        Assert.False( campaign.HasActiveRole( 42, CampaignMembershipRole.Player ) );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void CreateRejectsEmptyName( string name )
    {
        Assert.Throws<CampaignManagementException>( () => Campaign.Create( name, 42, _createdAtUtc ) );
    }

    [Fact]
    public void CreateRejectsNonUtcTimestamp()
    {
        DateTimeOffset nonUtcTimestamp = new DateTimeOffset( 2026, 7, 21, 10, 0, 0, TimeSpan.FromHours( 3 ) );

        Assert.Throws<CampaignManagementException>( () =>
            Campaign.Create( "Abomination Vaults", 42, nonUtcTimestamp ) );
    }

    [Fact]
    public void ArchiveStoresLifecycleStateForGameMaster()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );
        DateTimeOffset archivedAtUtc = _createdAtUtc.AddDays( 1 );

        campaign.Archive( 42, archivedAtUtc );

        Assert.Equal( CampaignStatus.Archived, campaign.Status );
        Assert.Equal( archivedAtUtc, campaign.ArchivedAtUtc );
    }

    [Fact]
    public void ArchiveRejectsUserWithoutGameMasterMembership()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );

        Assert.Throws<CampaignManagementException>( () =>
            campaign.Archive( 7, _createdAtUtc.AddDays( 1 ) ) );
    }
}
