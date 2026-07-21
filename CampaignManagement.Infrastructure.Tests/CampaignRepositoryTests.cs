using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CampaignManagement.Infrastructure.Tests.TestSupport;

namespace Pathfinder.CampaignManagement.Infrastructure.Tests;

public sealed class CampaignRepositoryTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 21, 10, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task CreateHandlerPersistsCampaignAndCreatorMembership()
    {
        await using CampaignManagementDbContext dbContext = CreateDbContext();
        CampaignRepository repository = new CampaignRepository( dbContext );
        CreateCampaignHandler handler = new CreateCampaignHandler(
            repository,
            new TestUnitOfWork( dbContext ),
            new CreateCampaignCommandValidator(),
            new FixedTimeProvider( _createdAtUtc ) );

        CampaignDto result = await handler.Handle(
            new CreateCampaignCommand( 42, "Abomination Vaults" ),
            CancellationToken.None );

        Campaign campaign = await dbContext.Campaigns
            .Include( item => item.Memberships )
            .SingleAsync();
        Assert.Equal( campaign.Id, result.Id );
        Assert.Equal( 42, campaign.CreatedByUserId );
        Assert.Equal( CampaignMembershipRole.GameMaster, Assert.Single( campaign.Memberships ).Role );
        Assert.Equal( new[] { CampaignMembershipRole.GameMaster }, result.Roles );
    }

    [Fact]
    public async Task GetForUserDoesNotReturnAnotherUsersCampaign()
    {
        await using CampaignManagementDbContext dbContext = CreateDbContext();
        dbContext.Campaigns.Add( Campaign.Create( "First", 42, _createdAtUtc ) );
        dbContext.Campaigns.Add( Campaign.Create( "Second", 7, _createdAtUtc ) );
        await dbContext.SaveChangesAsync();
        CampaignRepository repository = new CampaignRepository( dbContext );

        IReadOnlyCollection<Campaign> campaigns = await repository.GetForUserAsync(
            42,
            CancellationToken.None );

        Campaign campaign = Assert.Single( campaigns );
        Assert.Equal( "First", campaign.Name );
        Assert.True( campaign.HasActiveRole( 42, CampaignMembershipRole.GameMaster ) );
    }

    [Fact]
    public async Task ArchiveHandlerDoesNotRevealAnotherUsersCampaign()
    {
        await using CampaignManagementDbContext dbContext = CreateDbContext();
        dbContext.Campaigns.Add( Campaign.Create( "First", 42, _createdAtUtc ) );
        await dbContext.SaveChangesAsync();
        CampaignRepository repository = new CampaignRepository( dbContext );
        ArchiveCampaignHandler handler = new ArchiveCampaignHandler(
            repository,
            new TestUnitOfWork( dbContext ),
            new FixedTimeProvider( _createdAtUtc.AddDays( 1 ) ) );

        await Assert.ThrowsAsync<Pathfinder.CampaignManagement.Application.Exceptions.CampaignManagementApplicationException>( () =>
            handler.Handle( new ArchiveCampaignCommand( 7, 1 ), CancellationToken.None ) );
    }

    [Fact]
    public async Task ArchiveHandlerPersistsLifecycleStateForGameMaster()
    {
        await using CampaignManagementDbContext dbContext = CreateDbContext();
        dbContext.Campaigns.Add( Campaign.Create( "First", 42, _createdAtUtc ) );
        await dbContext.SaveChangesAsync();
        CampaignRepository repository = new CampaignRepository( dbContext );
        DateTimeOffset archivedAtUtc = _createdAtUtc.AddDays( 1 );
        ArchiveCampaignHandler handler = new ArchiveCampaignHandler(
            repository,
            new TestUnitOfWork( dbContext ),
            new FixedTimeProvider( archivedAtUtc ) );

        CampaignDto result = await handler.Handle(
            new ArchiveCampaignCommand( 42, 1 ),
            CancellationToken.None );

        Assert.Equal( CampaignStatus.Archived, result.Status );
        Assert.Equal( archivedAtUtc, result.ArchivedAtUtc );
        Campaign persistedCampaign = await dbContext.Campaigns.SingleAsync();
        Assert.Equal( CampaignStatus.Archived, persistedCampaign.Status );
        Assert.Equal( archivedAtUtc, persistedCampaign.ArchivedAtUtc );
    }

    [Fact]
    public async Task CreateHandlerRejectsInvalidNameBeforePersistence()
    {
        await using CampaignManagementDbContext dbContext = CreateDbContext();
        CreateCampaignHandler handler = new CreateCampaignHandler(
            new CampaignRepository( dbContext ),
            new TestUnitOfWork( dbContext ),
            new CreateCampaignCommandValidator(),
            new FixedTimeProvider( _createdAtUtc ) );

        await Assert.ThrowsAsync<ValidationException>( () =>
            handler.Handle( new CreateCampaignCommand( 42, " " ), CancellationToken.None ) );
        Assert.Empty( dbContext.Campaigns );
    }

    private static CampaignManagementDbContext CreateDbContext()
    {
        DbContextOptions<CampaignManagementDbContext> options =
            new DbContextOptionsBuilder<CampaignManagementDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CampaignManagementDbContext( options );
    }
}
