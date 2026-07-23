using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Shops;

namespace Pathfinder.Commerce.Infrastructure.Tests.Shops;

public sealed class ShopAdministrationServiceTests
{
    [Fact]
    public async Task GameMasterCreatesCampaignScopedSettlementAndShop()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CommerceDbContext dbContext = new CommerceDbContext( options );
        SettlementRepository repository = new SettlementRepository( dbContext );
        ShopAdministrationService service = new ShopAdministrationService(
            repository,
            new StubAccessPolicy( true ),
            new FixedTimeProvider() );

        SettlementDto settlement = await service.CreateSettlementAsync(
            new CreateSettlementRequest( 7, "Otari", 4, "Kortos", "Coastal", 11 ),
            CancellationToken.None );
        ShopDto shop = await service.CreateShopAsync(
            new CreateShopRequest( 7, settlement.Id, "Market", "General", 4, 11 ),
            CancellationToken.None );

        Assert.Equal( 7, settlement.CampaignId );
        Assert.Equal( 7, shop.CampaignId );
        Assert.Equal( settlement.Id, shop.SettlementId );
    }

    [Fact]
    public async Task NonGameMasterCannotCreateSettlement()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CommerceDbContext dbContext = new CommerceDbContext( options );
        ShopAdministrationService service = new ShopAdministrationService(
            new SettlementRepository( dbContext ),
            new StubAccessPolicy( false ),
            new FixedTimeProvider() );

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.CreateSettlementAsync(
                new CreateSettlementRequest( 7, "Otari", 4, String.Empty, String.Empty, 12 ),
                CancellationToken.None ) );
    }

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
            new DateTimeOffset( 2026, 7, 23, 8, 0, 0, TimeSpan.Zero );
    }
}
