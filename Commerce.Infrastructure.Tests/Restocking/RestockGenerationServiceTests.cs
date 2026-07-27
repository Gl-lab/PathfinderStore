using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Commerce.Infrastructure.Restocking;
using Pathfinder.Commerce.Infrastructure.Shops;

namespace Pathfinder.Commerce.Infrastructure.Tests.Restocking;

public sealed class RestockGenerationServiceTests
{
    [Fact]
    public async Task SameSeedAndPolicyVersionReturnSamePersistedRun()
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
        RestockPolicy policy = RestockPolicy.Create(
            7,
            shop.Id,
            "Weekly",
            2,
            new RestockPolicyConstraints(
                0,
                20,
                1000,
                RestockItemRarity.All,
                RestockItemAccess.All,
                RestockItemCategory.All ),
            new RestockSelectionWeights( 1, 1, 1 ),
            11,
            DateTimeOffset.UtcNow );
        dbContext.RestockPolicies.Add( policy );
        await dbContext.SaveChangesAsync();
        RestockGenerationService service = new RestockGenerationService(
            new RestockPolicyRepository( dbContext ),
            new RestockRunRepository( dbContext ),
            new SettlementRepository( dbContext ),
            new StubAccessPolicy(),
            new StubCatalogReader(),
            new DeterministicRestockSelector(),
            new FixedTimeProvider() );

        RestockRunDto first = await service.GenerateAsync(
            7,
            shop.Id,
            1,
            12345,
            11,
            CancellationToken.None );
        RestockRunDto second = await service.GenerateAsync(
            7,
            shop.Id,
            1,
            12345,
            11,
            CancellationToken.None );

        Assert.Equal( first.RunKey, second.RunKey );
        Assert.Equal( first.Lines, second.Lines );
        Assert.Single( dbContext.RestockRuns );
        Assert.Equal( 2, dbContext.RestockRunLines.Count() );
    }

    private sealed class StubAccessPolicy : ICommerceCampaignAccessPolicy
    {
        public Task<bool> IsGameMasterAsync(
            int campaignId,
            int actingUserId,
            CancellationToken cancellationToken ) => Task.FromResult( true );
    }

    private sealed class StubCatalogReader : ICommerceCatalogReader
    {
        public Task<bool> IsPublishedConfigurationAsync(
            int itemConfigurationId,
            int campaignId,
            CancellationToken cancellationToken ) => Task.FromResult( true );

        public Task<IReadOnlyCollection<CommerceCatalogCandidate>> GetRestockCandidatesAsync(
            int campaignId,
            CancellationToken cancellationToken ) =>
            Task.FromResult<IReadOnlyCollection<CommerceCatalogCandidate>>(
            [
                new CommerceCatalogCandidate( 10, 1, 200, 4, false ),
                new CommerceCatalogCandidate( 20, 2, 300, 1, false ),
                new CommerceCatalogCandidate( 30, 3, 400, 9, true ),
            ] );
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() =>
            new DateTimeOffset( 2026, 7, 27, 8, 0, 0, TimeSpan.Zero );
    }
}
