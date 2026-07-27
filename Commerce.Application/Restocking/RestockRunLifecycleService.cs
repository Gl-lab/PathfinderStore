using System.Buffers.Binary;
using System.Security.Cryptography;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Restocking;

public sealed class RestockRunLifecycleService
{
    private readonly IRestockRunRepository _runRepository;
    private readonly IShopOfferRepository _offerRepository;
    private readonly ISettlementRepository _settlementRepository;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;
    private readonly ICommerceRestockInventoryPort _inventoryPort;
    private readonly TimeProvider _timeProvider;

    public RestockRunLifecycleService(
        IRestockRunRepository runRepository,
        IShopOfferRepository offerRepository,
        ISettlementRepository settlementRepository,
        ICommerceCampaignAccessPolicy accessPolicy,
        ICommerceRestockInventoryPort inventoryPort,
        TimeProvider timeProvider )
    {
        _runRepository = runRepository;
        _offerRepository = offerRepository;
        _settlementRepository = settlementRepository;
        _accessPolicy = accessPolicy;
        _inventoryPort = inventoryPort;
        _timeProvider = timeProvider;
    }

    public async Task<RestockRunDto> GetAsync(
        int campaignId,
        int shopId,
        Guid runKey,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureAuthorizedShopAsync(
            campaignId,
            shopId,
            actingUserId,
            cancellationToken );
        RestockRun run = await GetRunAsync(
            campaignId,
            shopId,
            runKey,
            cancellationToken );
        return run.ToDto();
    }

    public async Task<RestockRunDto> ConfirmAsync(
        int campaignId,
        int shopId,
        Guid runKey,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureAuthorizedShopAsync(
            campaignId,
            shopId,
            actingUserId,
            cancellationToken );
        RestockRun run = await GetRunAsync(
            campaignId,
            shopId,
            runKey,
            cancellationToken );
        if ( run.Status == RestockRunStatus.Confirmed )
        {
            return run.ToDto();
        }

        if ( run.Status != RestockRunStatus.Preview )
        {
            throw new CommerceException( "Only a restock preview can be confirmed." );
        }

        await _inventoryPort.EnsureShopContainerAsync(
            campaignId,
            shopId,
            cancellationToken );
        DateTimeOffset confirmedAtUtc = _timeProvider.GetUtcNow();
        foreach ( RestockRunLine line in run.Lines.OrderBy( line => line.Sequence ) )
        {
            ShopOffer offer;
            Guid? itemInstanceKey = null;
            if ( line.Kind == RestockItemKind.Unique )
            {
                itemInstanceKey = CreateDeterministicInstanceKey( run.RunKey, line.Sequence );
                await _inventoryPort.EnsureUniqueShopStockAsync(
                    campaignId,
                    shopId,
                    line.ItemConfigurationId,
                    itemInstanceKey.Value,
                    confirmedAtUtc,
                    cancellationToken );
                offer = ShopOffer.CreateStockInstance(
                    campaignId,
                    shopId,
                    itemInstanceKey.Value,
                    line.Quantity,
                    line.UnitPriceCopper,
                    confirmedAtUtc );
            }
            else
            {
                offer = ShopOffer.CreateCatalog(
                    campaignId,
                    shopId,
                    line.ItemConfigurationId,
                    line.Quantity,
                    line.UnitPriceCopper,
                    confirmedAtUtc );
            }

            _offerRepository.Add( offer );
            line.Publish( offer.OfferKey, itemInstanceKey );
        }

        run.Confirm( actingUserId, confirmedAtUtc );
        await _runRepository.SaveChangesAsync( cancellationToken );
        return run.ToDto();
    }

    public async Task<RestockRunDto> RejectAsync(
        int campaignId,
        int shopId,
        Guid runKey,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        await EnsureAuthorizedShopAsync(
            campaignId,
            shopId,
            actingUserId,
            cancellationToken );
        RestockRun run = await GetRunAsync(
            campaignId,
            shopId,
            runKey,
            cancellationToken );
        if ( run.Status == RestockRunStatus.Rejected )
        {
            return run.ToDto();
        }

        if ( run.Status != RestockRunStatus.Preview )
        {
            throw new CommerceException( "Only a restock preview can be rejected." );
        }

        foreach ( RestockRunLine line in run.Lines.Where(
                     line => line.Kind == RestockItemKind.Unique ) )
        {
            await _inventoryPort.DiscardUniqueShopStockAsync(
                campaignId,
                shopId,
                line.ItemConfigurationId,
                CreateDeterministicInstanceKey( run.RunKey, line.Sequence ),
                cancellationToken );
        }

        run.Reject( actingUserId, _timeProvider.GetUtcNow() );
        await _runRepository.SaveChangesAsync( cancellationToken );
        return run.ToDto();
    }

    private async Task EnsureAuthorizedShopAsync(
        int campaignId,
        int shopId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        bool isGameMaster = await _accessPolicy.IsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        if ( !isGameMaster )
        {
            throw new UnauthorizedAccessException(
                "Only an active campaign Game Master can manage restock runs." );
        }

        Settlement settlement = await _settlementRepository.GetByShopAsync(
            shopId,
            cancellationToken ) ?? throw new CommerceException( "Shop was not found." );
        Shop shop = settlement.Shops.Single( candidate => candidate.Id == shopId );
        if ( shop.CampaignId != campaignId )
        {
            throw new CommerceException( "Shop does not belong to this campaign." );
        }
    }

    private async Task<RestockRun> GetRunAsync(
        int campaignId,
        int shopId,
        Guid runKey,
        CancellationToken cancellationToken )
    {
        RestockRun run = await _runRepository.GetByKeyAsync(
            runKey,
            cancellationToken ) ?? throw new CommerceException( "Restock run was not found." );
        if ( run.CampaignId != campaignId || run.ShopId != shopId )
        {
            throw new CommerceException( "Restock run does not belong to this campaign shop." );
        }

        return run;
    }

    private static Guid CreateDeterministicInstanceKey( Guid runKey, int sequence )
    {
        Span<byte> input = stackalloc byte[20];
        runKey.TryWriteBytes( input );
        BinaryPrimitives.WriteInt32LittleEndian( input[ 16.. ], sequence );
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData( input, hash );
        return new Guid( hash[ ..16 ] );
    }
}
