using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Shops;
using Pathfinder.Commerce.Infrastructure.Administration;
using Pathfinder.Commerce.Domain.Exceptions;

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
            new CommerceAdminOperationRepository( dbContext ),
            new StubAccessPolicy( true ),
            new FixedTimeProvider() );

        Guid settlementOperationId = Guid.NewGuid();
        SettlementDto settlement = await service.CreateSettlementAsync(
            new CreateSettlementRequest(
                7,
                settlementOperationId,
                "Otari",
                4,
                "Kortos",
                "Coastal",
                11 ),
            CancellationToken.None );
        SettlementDto replayedSettlement = await service.CreateSettlementAsync(
            new CreateSettlementRequest(
                7,
                settlementOperationId,
                "Otari",
                4,
                "Kortos",
                "Coastal",
                11 ),
            CancellationToken.None );
        Guid shopOperationId = Guid.NewGuid();
        ShopDto shop = await service.CreateShopAsync(
            new CreateShopRequest(
                7,
                settlement.Id,
                shopOperationId,
                "Market",
                "General",
                4,
                11 ),
            CancellationToken.None );
        ShopDto replayedShop = await service.CreateShopAsync(
            new CreateShopRequest(
                7,
                settlement.Id,
                shopOperationId,
                "Market",
                "General",
                4,
                11 ),
            CancellationToken.None );

        Assert.Equal( 7, settlement.CampaignId );
        Assert.Equal( settlement.Id, replayedSettlement.Id );
        Assert.Equal( 7, shop.CampaignId );
        Assert.Equal( shop.Id, replayedShop.Id );
        Assert.Equal( settlement.Id, shop.SettlementId );
        Assert.Single( dbContext.Settlements );
        Assert.Single( dbContext.Shops );
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
            new CommerceAdminOperationRepository( dbContext ),
            new StubAccessPolicy( false ),
            new FixedTimeProvider() );

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.CreateSettlementAsync(
                new CreateSettlementRequest(
                    7,
                    Guid.NewGuid(),
                    "Otari",
                    4,
                    String.Empty,
                    String.Empty,
                    12 ),
                CancellationToken.None ) );
    }

    [Fact]
    public async Task ReusedOperationIdRejectsDifferentAdminPayload()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CommerceDbContext dbContext = new CommerceDbContext( options );
        ShopAdministrationService service = new ShopAdministrationService(
            new SettlementRepository( dbContext ),
            new CommerceAdminOperationRepository( dbContext ),
            new StubAccessPolicy( true ),
            new FixedTimeProvider() );
        Guid operationId = Guid.NewGuid();

        await service.CreateSettlementAsync(
            new CreateSettlementRequest(
                7,
                operationId,
                "Otari",
                4,
                String.Empty,
                String.Empty,
                11 ),
            CancellationToken.None );

        await Assert.ThrowsAsync<CommerceException>(
            () => service.CreateSettlementAsync(
                new CreateSettlementRequest(
                    7,
                    operationId,
                    "Absalom",
                    20,
                    String.Empty,
                    String.Empty,
                    11 ),
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
