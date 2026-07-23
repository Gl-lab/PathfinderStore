using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Shops;

public sealed class ShopAdministrationService
{
    private readonly ISettlementRepository _repository;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public ShopAdministrationService(
        ISettlementRepository repository,
        ICommerceCampaignAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
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
        Settlement settlement = Settlement.Create(
            request.CampaignId,
            request.Name,
            request.Level,
            request.Region,
            request.Traits,
            _timeProvider.GetUtcNow() );
        _repository.Add( settlement );
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
