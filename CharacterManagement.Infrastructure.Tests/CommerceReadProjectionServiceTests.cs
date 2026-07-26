using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class CommerceReadProjectionServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 26, 17, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task WalletProjectionReturnsBalancesAndNewestLedgerFirst()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        Wallet wallet = Wallet.Create( 42, 101, _now );
        wallet.ApplyAdjustment(
            Guid.NewGuid(),
            1000,
            "Initial funds.",
            7,
            _now.AddMinutes( 1 ) );
        wallet.ApplyAdjustment(
            Guid.NewGuid(),
            -100,
            "Supplies.",
            7,
            _now.AddMinutes( 2 ) );
        commerceDbContext.Wallets.Add( wallet );
        await commerceDbContext.SaveChangesAsync();
        CommerceReadProjectionService service = new CommerceReadProjectionService(
            commerceDbContext,
            campaignDbContext,
            new StubBuyerAccessPolicy( true ) );

        WalletDto result = await service.GetWalletAsync(
            42,
            101,
            7,
            CancellationToken.None );

        Assert.Equal( 900, result.BalanceCopper );
        Assert.Equal( 900, result.AvailableCopper );
        Assert.Equal( 2, result.Version );
        Assert.Equal(
            "Supplies.",
            result.Entries.First().Description );
    }

    [Fact]
    public async Task SettlementProjectionIsCampaignScopedAndIncludesShops()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _now );
        campaignDbContext.Campaigns.Add( campaign );
        await campaignDbContext.SaveChangesAsync();
        Settlement settlement = Settlement.Create(
            campaign.Id,
            "Otari",
            4,
            "Isle of Kortos",
            "Town",
            _now );
        settlement.AddShop(
            "Wrin's Wonders",
            "Curiosities",
            4,
            _now.AddMinutes( 1 ) );
        Settlement other = Settlement.Create(
            campaign.Id + 1,
            "Absalom",
            20,
            "Isle of Kortos",
            "Metropolis",
            _now );
        commerceDbContext.Settlements.AddRange( settlement, other );
        await commerceDbContext.SaveChangesAsync();
        CommerceReadProjectionService service = new CommerceReadProjectionService(
            commerceDbContext,
            campaignDbContext,
            new StubBuyerAccessPolicy( false ) );

        IReadOnlyCollection<SettlementDto> result = await service.GetSettlementsAsync(
            campaign.Id,
            42,
            CancellationToken.None );

        SettlementDto dto = Assert.Single( result );
        Assert.Equal( "Otari", dto.Name );
        Assert.Equal( "Wrin's Wonders", Assert.Single( dto.Shops ).Name );
    }

    [Fact]
    public async Task WalletProjectionRejectsUncontrolledCharacter()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        CommerceReadProjectionService service = new CommerceReadProjectionService(
            commerceDbContext,
            campaignDbContext,
            new StubBuyerAccessPolicy( false ) );

        await Assert.ThrowsAsync<CommerceReadAccessDeniedException>(
            () => service.GetWalletAsync(
                42,
                101,
                7,
                CancellationToken.None ) );
    }

    [Fact]
    public async Task ControlledCharacterWithoutWalletReceivesZeroProjection()
    {
        await using CommerceDbContext commerceDbContext = CreateCommerceContext();
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        CommerceReadProjectionService service = new CommerceReadProjectionService(
            commerceDbContext,
            campaignDbContext,
            new StubBuyerAccessPolicy( true ) );

        WalletDto result = await service.GetWalletAsync(
            42,
            101,
            7,
            CancellationToken.None );

        Assert.Equal( 0, result.AvailableCopper );
        Assert.Empty( result.Entries );
    }

    private static CommerceDbContext CreateCommerceContext()
    {
        DbContextOptions<CommerceDbContext> options =
            new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CommerceDbContext( options );
    }

    private static CampaignManagementDbContext CreateCampaignContext()
    {
        DbContextOptions<CampaignManagementDbContext> options =
            new DbContextOptionsBuilder<CampaignManagementDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CampaignManagementDbContext( options );
    }

    private sealed class StubBuyerAccessPolicy : ICommerceBuyerAccessPolicy
    {
        private readonly bool _controlsCharacter;

        public StubBuyerAccessPolicy( bool controlsCharacter )
        {
            _controlsCharacter = controlsCharacter;
        }

        public Task<bool> ControlsCharacterAsync(
            int campaignId,
            int actingUserId,
            int characterId,
            CancellationToken cancellationToken ) => Task.FromResult( _controlsCharacter );
    }
}