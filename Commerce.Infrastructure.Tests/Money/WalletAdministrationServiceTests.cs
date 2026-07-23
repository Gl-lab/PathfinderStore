using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Money;

namespace Pathfinder.Commerce.Infrastructure.Tests.Money;

public sealed class WalletAdministrationServiceTests
{
    [Fact]
    public async Task RepeatedAdjustmentIsPersistedOnce()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CommerceDbContext dbContext = new CommerceDbContext( options );
        WalletAdministrationService service = new WalletAdministrationService(
            new WalletRepository( dbContext ),
            new StubAccessPolicy(),
            new FixedTimeProvider() );
        Guid operationId = Guid.NewGuid();

        await service.AdjustAsync(
            7,
            19,
            operationId,
            1000,
            "Starting funds",
            11,
            CancellationToken.None );
        WalletDto repeated = await service.AdjustAsync(
            7,
            19,
            operationId,
            1000,
            "Starting funds",
            11,
            CancellationToken.None );

        Assert.Equal( 1000, repeated.BalanceCopper );
        Assert.Single( repeated.Entries );
        Assert.Equal( 1, await dbContext.WalletLedgerEntries.CountAsync() );
    }

    private sealed class StubAccessPolicy : ICommerceCampaignAccessPolicy
    {
        public Task<bool> IsGameMasterAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken ) => Task.FromResult( true );
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() =>
            new DateTimeOffset( 2026, 7, 23, 10, 0, 0, TimeSpan.Zero );
    }
}
