using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Restocking;
using Pathfinder.Commerce.Infrastructure.Shops;

namespace Pathfinder.Commerce.Infrastructure.Tests.Restocking;

public sealed class RestockPolicyAdministrationServiceTests
{
    [Fact]
    public async Task GameMasterCreatesAndRevisesPolicyWithoutLosingHistory()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CommerceDbContext dbContext = new CommerceDbContext( options );
        Settlement settlement = Settlement.Create(
            7,
            "Otari",
            4,
            String.Empty,
            String.Empty,
            DateTimeOffset.UtcNow );
        Shop shop = settlement.AddShop( "Market", "General", 4, DateTimeOffset.UtcNow );
        dbContext.Settlements.Add( settlement );
        await dbContext.SaveChangesAsync();
        RestockPolicyAdministrationService service = CreateService( dbContext, true );

        RestockPolicyDto created = await service.CreateAsync(
            new CreateRestockPolicyRequest(
                7,
                shop.Id,
                "Weekly",
                8,
                DefaultConstraints(),
                DefaultWeights(),
                11 ),
            CancellationToken.None );
        RestockPolicyDto revised = await service.ReviseAsync(
            new ReviseRestockPolicyRequest(
                7,
                shop.Id,
                created.CurrentVersion,
                12,
                DefaultConstraints(),
                new RestockSelectionWeights( 3, 2, 1 ),
                11 ),
            CancellationToken.None );

        Assert.Equal( 2, revised.CurrentVersion );
        Assert.Equal( new[] { 8, 12 }, revised.Revisions
            .Select( revision => revision.TargetOfferCount )
            .ToArray() );
    }

    [Fact]
    public async Task NonGameMasterCannotCreatePolicy()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CommerceDbContext dbContext = new CommerceDbContext( options );
        RestockPolicyAdministrationService service = CreateService( dbContext, false );

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.CreateAsync(
                new CreateRestockPolicyRequest(
                    7,
                    11,
                    "Weekly",
                    8,
                    DefaultConstraints(),
                    DefaultWeights(),
                    12 ),
                CancellationToken.None ) );
    }

    private static RestockPolicyAdministrationService CreateService(
        CommerceDbContext dbContext,
        bool isGameMaster ) => new RestockPolicyAdministrationService(
        new RestockPolicyRepository( dbContext ),
        new SettlementRepository( dbContext ),
        new StubAccessPolicy( isGameMaster ),
        new FixedTimeProvider() );

    private static RestockPolicyConstraints DefaultConstraints() =>
        new RestockPolicyConstraints(
            0,
            20,
            10000,
            RestockItemRarity.All,
            RestockItemAccess.All,
            RestockItemCategory.All );

    private static RestockSelectionWeights DefaultWeights() =>
        new RestockSelectionWeights( 1, 1, 0 );

    private sealed class StubAccessPolicy : ICommerceCampaignAccessPolicy
    {
        private readonly bool _isGameMaster;

        public StubAccessPolicy( bool isGameMaster )
        {
            _isGameMaster = isGameMaster;
        }

        public Task<bool> IsGameMasterAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken ) => Task.FromResult( _isGameMaster );
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() =>
            new DateTimeOffset( 2026, 7, 27, 8, 0, 0, TimeSpan.Zero );
    }
}
