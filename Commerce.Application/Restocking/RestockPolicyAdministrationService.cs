using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Restocking;

public sealed class RestockPolicyAdministrationService
{
    private readonly IRestockPolicyRepository _repository;
    private readonly ISettlementRepository _settlementRepository;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public RestockPolicyAdministrationService(
        IRestockPolicyRepository repository,
        ISettlementRepository settlementRepository,
        ICommerceCampaignAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _settlementRepository = settlementRepository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<RestockPolicyDto> CreateAsync(
        CreateRestockPolicyRequest request,
        CancellationToken cancellationToken )
    {
        await EnsureAuthorizedShopAsync(
            request.CampaignId,
            request.ShopId,
            request.ActingUserId,
            cancellationToken );
        RestockPolicy? existing = await _repository.GetByShopAsync(
            request.ShopId,
            cancellationToken );
        if ( existing is not null )
        {
            throw new CommerceException( "Shop already has a restock policy." );
        }

        RestockPolicy policy = RestockPolicy.Create(
            request.CampaignId,
            request.ShopId,
            request.Name,
            request.TargetOfferCount,
            request.Constraints,
            request.Weights,
            request.ActingUserId,
            _timeProvider.GetUtcNow() );
        _repository.Add( policy );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( policy );
    }

    public async Task<RestockPolicyDto> ReviseAsync(
        ReviseRestockPolicyRequest request,
        CancellationToken cancellationToken )
    {
        await EnsureAuthorizedShopAsync(
            request.CampaignId,
            request.ShopId,
            request.ActingUserId,
            cancellationToken );
        RestockPolicy policy = await _repository.GetByShopAsync(
            request.ShopId,
            cancellationToken ) ?? throw new CommerceException( "Restock policy was not found." );
        if ( policy.CampaignId != request.CampaignId )
        {
            throw new CommerceException( "Restock policy does not belong to this campaign." );
        }

        policy.Revise(
            request.ExpectedVersion,
            request.TargetOfferCount,
            request.Constraints,
            request.Weights,
            request.ActingUserId,
            _timeProvider.GetUtcNow() );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( policy );
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
                "Only an active campaign Game Master can manage restock policies." );
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

    private static RestockPolicyDto ToDto( RestockPolicy policy ) => new RestockPolicyDto(
        policy.Id,
        policy.CampaignId,
        policy.ShopId,
        policy.Name,
        policy.CurrentVersion,
        policy.Revisions
            .OrderBy( revision => revision.Version )
            .Select( revision => new RestockPolicyRevisionDto(
                revision.Version,
                revision.TargetOfferCount,
                revision.MinimumItemLevel,
                revision.MaximumItemLevel,
                revision.BudgetCopper,
                revision.AllowedRarities,
                revision.AllowedAccess,
                revision.AllowedCategories,
                revision.ConsumableWeight,
                revision.PermanentWeight,
                revision.UniqueWeight,
                revision.CreatedByUserId,
                revision.CreatedAtUtc ) )
            .ToArray() );
}
