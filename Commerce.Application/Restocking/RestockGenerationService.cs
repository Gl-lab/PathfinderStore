using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Restocking;

public sealed class RestockGenerationService
{
    private readonly IRestockPolicyRepository _policyRepository;
    private readonly IRestockRunRepository _runRepository;
    private readonly ISettlementRepository _settlementRepository;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;
    private readonly ICommerceCatalogReader _catalogReader;
    private readonly DeterministicRestockSelector _selector;
    private readonly TimeProvider _timeProvider;

    public RestockGenerationService(
        IRestockPolicyRepository policyRepository,
        IRestockRunRepository runRepository,
        ISettlementRepository settlementRepository,
        ICommerceCampaignAccessPolicy accessPolicy,
        ICommerceCatalogReader catalogReader,
        DeterministicRestockSelector selector,
        TimeProvider timeProvider )
    {
        _policyRepository = policyRepository;
        _runRepository = runRepository;
        _settlementRepository = settlementRepository;
        _accessPolicy = accessPolicy;
        _catalogReader = catalogReader;
        _selector = selector;
        _timeProvider = timeProvider;
    }

    public async Task<RestockRunDto> GenerateAsync(
        int campaignId,
        int shopId,
        int policyVersion,
        long seed,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Shop shop = await GetAuthorizedShopAsync(
            campaignId,
            shopId,
            actingUserId,
            cancellationToken );
        RestockPolicy policy = await _policyRepository.GetByShopAsync(
            shopId,
            cancellationToken ) ?? throw new CommerceException( "Restock policy was not found." );
        if ( policy.CampaignId != campaignId )
        {
            throw new CommerceException( "Restock policy does not belong to this campaign." );
        }

        RestockPolicyRevision revision = policy.GetRevision( policyVersion );
        RestockRun? existing = await _runRepository.GetByIdentityAsync(
            shopId,
            policy.Id,
            policyVersion,
            seed,
            cancellationToken );
        if ( existing is not null )
        {
            return ToDto( existing );
        }

        IReadOnlyCollection<CommerceCatalogCandidate> catalogCandidates =
            await _catalogReader.GetRestockCandidatesAsync( campaignId, cancellationToken );
        IReadOnlyCollection<RestockCandidate> candidates = catalogCandidates
            .Select( candidate => ToCandidate( shop, candidate ) )
            .ToArray();
        IReadOnlyCollection<RestockCandidate> selected = _selector.Select(
            revision,
            seed,
            candidates );
        RestockRun run = RestockRun.CreatePreview(
            campaignId,
            shopId,
            policy.Id,
            policyVersion,
            seed,
            actingUserId,
            _timeProvider.GetUtcNow(),
            selected );
        _runRepository.Add( run );
        await _runRepository.SaveChangesAsync( cancellationToken );
        return ToDto( run );
    }

    private async Task<Shop> GetAuthorizedShopAsync(
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
                "Only an active campaign Game Master can generate restock previews." );
        }

        Settlement settlement = await _settlementRepository.GetByShopAsync(
            shopId,
            cancellationToken ) ?? throw new CommerceException( "Shop was not found." );
        Shop shop = settlement.Shops.Single( candidate => candidate.Id == shopId );
        if ( shop.CampaignId != campaignId )
        {
            throw new CommerceException( "Shop does not belong to this campaign." );
        }

        return shop;
    }

    private static RestockCandidate ToCandidate(
        Shop shop,
        CommerceCatalogCandidate candidate )
    {
        RestockItemCategory category = ToCategory( candidate.PrimaryCategory );
        bool isConsumable = category is RestockItemCategory.Consumable or
            RestockItemCategory.Ammunition;
        RestockItemKind kind = candidate.IsCampaignScoped
            ? RestockItemKind.Unique
            : isConsumable
                ? RestockItemKind.Consumable
                : RestockItemKind.Permanent;
        return new RestockCandidate(
            candidate.ItemConfigurationId,
            candidate.Level,
            shop.CalculateCatalogPrice( candidate.BasePriceCopper ),
            candidate.IsCampaignScoped ? RestockItemRarity.Unique : RestockItemRarity.Common,
            candidate.IsCampaignScoped ? RestockItemAccess.Campaign : RestockItemAccess.Global,
            category,
            kind );
    }

    private static RestockItemCategory ToCategory( int value ) => value switch
    {
        1 => RestockItemCategory.Weapon,
        2 => RestockItemCategory.Armor,
        3 => RestockItemCategory.Shield,
        4 => RestockItemCategory.Consumable,
        5 => RestockItemCategory.Ammunition,
        6 => RestockItemCategory.Rune,
        7 => RestockItemCategory.Tool,
        8 => RestockItemCategory.Container,
        9 => RestockItemCategory.OtherEquipment,
        _ => 0,
    };

    private static RestockRunDto ToDto( RestockRun run ) => new RestockRunDto(
        run.RunKey,
        run.CampaignId,
        run.ShopId,
        run.RestockPolicyId,
        run.PolicyVersion,
        run.Seed,
        run.Status,
        run.TotalPriceCopper,
        run.Lines
            .OrderBy( line => line.Sequence )
            .Select( line => new RestockRunLineDto(
                line.Sequence,
                line.ItemConfigurationId,
                line.Quantity,
                line.UnitPriceCopper,
                line.Kind ) )
            .ToArray() );
}
