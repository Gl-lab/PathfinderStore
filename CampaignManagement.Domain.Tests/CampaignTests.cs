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

    [Fact]
    public void AcceptInvitationAddsPlayerMembership()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );
        CampaignInvitation invitation = campaign.Invite( 42, 7, _createdAtUtc.AddHours( 1 ) );

        campaign.AcceptInvitation( invitation.Id, 7, _createdAtUtc.AddHours( 2 ) );

        Assert.Equal( CampaignInvitationStatus.Accepted, invitation.Status );
        Assert.True( campaign.HasActiveRole( 7, CampaignMembershipRole.Player ) );
        Assert.False( campaign.HasActiveRole( 7, CampaignMembershipRole.GameMaster ) );
    }

    [Fact]
    public void InviteRejectsDuplicatePendingInvitation()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );
        campaign.Invite( 42, 7, _createdAtUtc.AddHours( 1 ) );

        Assert.Throws<CampaignManagementException>( () =>
            campaign.Invite( 42, 7, _createdAtUtc.AddHours( 2 ) ) );
    }

    [Fact]
    public void InvitationCanOnlyBeAnsweredByInvitedUser()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );
        CampaignInvitation invitation = campaign.Invite( 42, 7, _createdAtUtc.AddHours( 1 ) );

        Assert.Throws<CampaignManagementException>( () =>
            campaign.AcceptInvitation( invitation.Id, 8, _createdAtUtc.AddHours( 2 ) ) );
    }

    [Fact]
    public void LastGameMasterCannotLeaveOrLoseRole()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );

        Assert.Throws<CampaignManagementException>( () => campaign.Leave( 42 ) );
        Assert.Throws<CampaignManagementException>( () =>
            campaign.RevokeRole( 42, 42, CampaignMembershipRole.GameMaster ) );
    }

    [Fact]
    public void GameMasterCanTransferResponsibilityAndLeave()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );
        CampaignInvitation invitation = campaign.Invite( 42, 7, _createdAtUtc.AddHours( 1 ) );
        campaign.AcceptInvitation( invitation.Id, 7, _createdAtUtc.AddHours( 2 ) );
        campaign.AssignRole( 42, 7, CampaignMembershipRole.GameMaster, _createdAtUtc.AddHours( 3 ) );

        campaign.Leave( 42 );

        Assert.False( campaign.HasActiveRole( 42, CampaignMembershipRole.GameMaster ) );
        Assert.True( campaign.HasActiveRole( 7, CampaignMembershipRole.GameMaster ) );
        Assert.True( campaign.HasActiveRole( 7, CampaignMembershipRole.Player ) );
    }

    [Fact]
    public void ArchivedCampaignRejectsMembershipChanges()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );
        campaign.Archive( 42, _createdAtUtc.AddHours( 1 ) );

        Assert.Throws<CampaignManagementException>( () =>
            campaign.Invite( 42, 7, _createdAtUtc.AddHours( 2 ) ) );
    }

    [Fact]
    public void GameMasterCreatesOnlyOneActiveParty()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );

        CampaignParty party = campaign.CreateParty( 42, "Heroes", _createdAtUtc.AddHours( 1 ) );

        Assert.Equal( "Heroes", party.Name );
        Assert.Equal( CampaignPartyStatus.Active, party.Status );
        Assert.Equal( CampaignPartyStorageAccessPolicy.Unconfigured, party.Storage.AccessPolicy );
        Assert.Equal( _createdAtUtc.AddHours( 1 ), party.Storage.CreatedAtUtc );
        Assert.Empty( party.Characters );
        Assert.Throws<CampaignManagementException>( () =>
            campaign.CreateParty( 42, "Second", _createdAtUtc.AddHours( 2 ) ) );
    }

    [Fact]
    public void PlayerAssignsOwnCharacterToActiveParty()
    {
        Campaign campaign = CreateCampaignWithPlayer();
        campaign.CreateParty( 42, "Heroes", _createdAtUtc.AddHours( 3 ) );

        CampaignPartyCharacter character = campaign.AssignCharacterToActiveParty(
            7,
            101,
            7,
            _createdAtUtc.AddHours( 4 ) );

        Assert.Equal( 101, character.CharacterId );
        Assert.Equal( 7, character.ControlledByUserId );
    }

    [Fact]
    public void PlayerCannotAssignCharacterForAnotherPlayer()
    {
        Campaign campaign = CreateCampaignWithPlayer();
        CampaignInvitation secondInvitation = campaign.Invite( 42, 8, _createdAtUtc.AddHours( 3 ) );
        campaign.AcceptInvitation( secondInvitation.Id, 8, _createdAtUtc.AddHours( 4 ) );
        campaign.CreateParty( 42, "Heroes", _createdAtUtc.AddHours( 5 ) );

        Assert.Throws<CampaignManagementException>( () =>
            campaign.AssignCharacterToActiveParty(
                7,
                101,
                8,
                _createdAtUtc.AddHours( 6 ) ) );
    }

    [Fact]
    public void PartyRejectsDuplicateCharacter()
    {
        Campaign campaign = CreateCampaignWithPlayer();
        campaign.CreateParty( 42, "Heroes", _createdAtUtc.AddHours( 3 ) );
        campaign.AssignCharacterToActiveParty( 7, 101, 7, _createdAtUtc.AddHours( 4 ) );

        Assert.Throws<CampaignManagementException>( () =>
            campaign.AssignCharacterToActiveParty( 7, 101, 7, _createdAtUtc.AddHours( 5 ) ) );
    }

    private static Campaign CreateCampaignWithPlayer()
    {
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _createdAtUtc );
        CampaignInvitation invitation = campaign.Invite( 42, 7, _createdAtUtc.AddHours( 1 ) );
        campaign.AcceptInvitation( invitation.Id, 7, _createdAtUtc.AddHours( 2 ) );
        return campaign;
    }
}
