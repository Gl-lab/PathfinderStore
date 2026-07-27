using Pathfinder.Commerce.Application.Administration;
using Pathfinder.Commerce.Domain.Administration;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Shops;

public sealed class ShopAdministrationService
{
    private readonly ISettlementRepository _repository;
    private readonly ICommerceAdminOperationRepository _operationRepository;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public ShopAdministrationService(
        ISettlementRepository repository,
        ICommerceAdminOperationRepository operationRepository,
        ICommerceCampaignAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _operationRepository = operationRepository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<SettlementDto> CreateSettlementAsync(
        CreateSettlementRequest request,
        CancellationToken cancellationToken )
    {
        await EnsureGameMasterAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        CommerceAdminOperationSupport.EnsureOperationId( request.OperationId );
        const string actionKind = "CreateSettlement";
        string payloadHash = CommerceAdminOperationSupport.HashPayload( new
        {
            request.Name,
            request.Level,
            request.Region,
            request.Traits,
        } );
        CommerceAdminOperation? replay = await _operationRepository.GetAsync(
            request.CampaignId,
            request.OperationId,
            cancellationToken );
        if ( replay != null )
        {
            replay.EnsureReplayMatches( actionKind, payloadHash );
            Settlement existing = await _repository.GetAsync(
                replay.SettlementId!.Value,
                cancellationToken ) ?? throw new CommerceException(
                "Replayed settlement was not found." );
            return existing.ToDto();
        }

        Settlement settlement = Settlement.Create(
            request.CampaignId,
            request.Name,
            request.Level,
            request.Region,
            request.Traits,
            _timeProvider.GetUtcNow() );
        _repository.Add( settlement );
        _operationRepository.Add( CommerceAdminOperationSupport.Create(
            request.CampaignId,
            request.OperationId,
            actionKind,
            payloadHash,
            request.ActingUserId,
            _timeProvider.GetUtcNow(),
            settlement: settlement ) );
        await _repository.SaveChangesAsync( cancellationToken );
        return settlement.ToDto();
    }

    public async Task<ShopDto> CreateShopAsync(
        CreateShopRequest request,
        CancellationToken cancellationToken )
    {
        await EnsureGameMasterAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        CommerceAdminOperationSupport.EnsureOperationId( request.OperationId );
        const string actionKind = "CreateShop";
        string payloadHash = CommerceAdminOperationSupport.HashPayload( new
        {
            request.SettlementId,
            request.Name,
            request.Specialization,
            request.ShopLevel,
        } );
        CommerceAdminOperation? replay = await _operationRepository.GetAsync(
            request.CampaignId,
            request.OperationId,
            cancellationToken );
        if ( replay != null )
        {
            replay.EnsureReplayMatches( actionKind, payloadHash );
            Settlement existingSettlement = await _repository.GetByShopAsync(
                replay.ShopId!.Value,
                cancellationToken ) ?? throw new CommerceException(
                "Replayed shop was not found." );
            return existingSettlement.Shops
                .Single( candidate => candidate.Id == replay.ShopId.Value )
                .ToDto();
        }

        Settlement settlement = await _repository.GetAsync(
            request.SettlementId,
            cancellationToken ) ?? throw new CommerceException( "Settlement was not found." );
        if ( settlement.CampaignId != request.CampaignId )
        {
            throw new CommerceException( "Settlement does not belong to this campaign." );
        }

        Shop shop = settlement.AddShop(
            request.Name,
            request.Specialization,
            request.ShopLevel,
            _timeProvider.GetUtcNow() );
        _operationRepository.Add( CommerceAdminOperationSupport.Create(
            request.CampaignId,
            request.OperationId,
            actionKind,
            payloadHash,
            request.ActingUserId,
            _timeProvider.GetUtcNow(),
            settlement: settlement,
            shop: shop ) );
        await _repository.SaveChangesAsync( cancellationToken );
        return shop.ToDto();
    }

    public async Task<ShopDto> UpdatePricingPolicyAsync(
        UpdateShopPricingPolicyRequest request,
        CancellationToken cancellationToken )
    {
        await EnsureGameMasterAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        CommerceAdminOperationSupport.EnsureOperationId( request.OperationId );
        const string actionKind = "UpdatePricingPolicy";
        string payloadHash = CommerceAdminOperationSupport.HashPayload( new
        {
            request.ShopId,
            request.CatalogPricePercent,
            request.BuybackPricePercent,
        } );
        CommerceAdminOperation? replay = await _operationRepository.GetAsync(
            request.CampaignId,
            request.OperationId,
            cancellationToken );
        if ( replay != null )
        {
            replay.EnsureReplayMatches( actionKind, payloadHash );
            Settlement existingSettlement = await _repository.GetByShopAsync(
                replay.ShopId!.Value,
                cancellationToken ) ?? throw new CommerceException(
                "Replayed shop was not found." );
            return existingSettlement.Shops
                .Single( candidate => candidate.Id == replay.ShopId.Value )
                .ToDto();
        }

        Settlement settlement = await _repository.GetByShopAsync(
            request.ShopId,
            cancellationToken ) ?? throw new CommerceException( "Shop was not found." );
        if ( settlement.CampaignId != request.CampaignId )
        {
            throw new CommerceException( "Shop does not belong to this campaign." );
        }

        Shop shop = settlement.Shops.Single( candidate => candidate.Id == request.ShopId );
        shop.SetPricingPolicy(
            request.CatalogPricePercent,
            request.BuybackPricePercent );
        _operationRepository.Add( CommerceAdminOperationSupport.Create(
            request.CampaignId,
            request.OperationId,
            actionKind,
            payloadHash,
            request.ActingUserId,
            _timeProvider.GetUtcNow(),
            settlement: settlement,
            shop: shop ) );
        await _repository.SaveChangesAsync( cancellationToken );
        return shop.ToDto();
    }

    private async Task EnsureGameMasterAsync(
        int campaignId,
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
                "Only an active campaign Game Master can manage shops." );
        }
    }
}
