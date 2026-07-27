using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/commerce-admin/campaigns/{campaignId:int}" )]
public sealed class CommerceAdminController : AuthorizedController
{
    private readonly ShopAdministrationService _service;
    private readonly ShopOfferAdministrationService _offerService;
    private readonly WalletAdministrationService _walletService;
    private readonly RestockPolicyAdministrationService _restockPolicyService;
    private readonly RestockGenerationService _restockGenerationService;
    private readonly RestockRunLifecycleService _restockRunLifecycleService;

    public CommerceAdminController(
        ShopAdministrationService service,
        ShopOfferAdministrationService offerService,
        WalletAdministrationService walletService,
        RestockPolicyAdministrationService restockPolicyService,
        RestockGenerationService restockGenerationService,
        RestockRunLifecycleService restockRunLifecycleService )
    {
        _service = service;
        _offerService = offerService;
        _walletService = walletService;
        _restockPolicyService = restockPolicyService;
        _restockGenerationService = restockGenerationService;
        _restockRunLifecycleService = restockRunLifecycleService;
    }

    [HttpGet( "shops/{shopId:int}/restock-runs/{runKey:guid}" )]
    public async Task<ActionResult<RestockRunDto>> GetRestockRun(
        int campaignId,
        int shopId,
        Guid runKey,
        CancellationToken cancellationToken )
    {
        return await ExecuteRestockRunAction(
            () => _restockRunLifecycleService.GetAsync(
                campaignId,
                shopId,
                runKey,
                CurrentUserId(),
                cancellationToken ) );
    }

    [HttpPost( "shops/{shopId:int}/restock-runs/{runKey:guid}/confirm" )]
    public async Task<ActionResult<RestockRunDto>> ConfirmRestockRun(
        int campaignId,
        int shopId,
        Guid runKey,
        CancellationToken cancellationToken )
    {
        return await ExecuteRestockRunAction(
            () => _restockRunLifecycleService.ConfirmAsync(
                campaignId,
                shopId,
                runKey,
                CurrentUserId(),
                cancellationToken ) );
    }

    [HttpPost( "shops/{shopId:int}/restock-runs/{runKey:guid}/reject" )]
    public async Task<ActionResult<RestockRunDto>> RejectRestockRun(
        int campaignId,
        int shopId,
        Guid runKey,
        CancellationToken cancellationToken )
    {
        return await ExecuteRestockRunAction(
            () => _restockRunLifecycleService.RejectAsync(
                campaignId,
                shopId,
                runKey,
                CurrentUserId(),
                cancellationToken ) );
    }

    private static async Task<ActionResult<RestockRunDto>> ExecuteRestockRunAction(
        Func<Task<RestockRunDto>> action )
    {
        try
        {
            return new OkObjectResult( await action() );
        }
        catch ( UnauthorizedAccessException )
        {
            return new ForbidResult();
        }
        catch ( CommerceException exception )
        {
            return new BadRequestObjectResult( MapError( exception.Message ) );
        }
    }

    [HttpPost( "shops/{shopId:int}/restock-runs" )]
    public async Task<ActionResult<RestockRunDto>> GenerateRestockRun(
        int campaignId,
        int shopId,
        [FromBody] GenerateRestockRunApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            RestockRunDto result = await _restockGenerationService.GenerateAsync(
                campaignId,
                shopId,
                request.PolicyVersion,
                request.Seed,
                CurrentUserId(),
                cancellationToken );
            return Ok( result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "shops/{shopId:int}/restock-policy" )]
    public async Task<ActionResult<RestockPolicyDto>> CreateRestockPolicy(
        int campaignId,
        int shopId,
        [FromBody] CreateRestockPolicyApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            RestockPolicyDto result = await _restockPolicyService.CreateAsync(
                new CreateRestockPolicyRequest(
                    campaignId,
                    shopId,
                    request.Name,
                    request.TargetOfferCount,
                    request.ToConstraints(),
                    request.ToWeights(),
                    CurrentUserId() ),
                cancellationToken );
            return Created(
                $"api/commerce-admin/campaigns/{campaignId}/shops/{shopId}/restock-policy",
                result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "shops/{shopId:int}/restock-policy/revisions" )]
    public async Task<ActionResult<RestockPolicyDto>> ReviseRestockPolicy(
        int campaignId,
        int shopId,
        [FromBody] ReviseRestockPolicyApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            RestockPolicyDto result = await _restockPolicyService.ReviseAsync(
                new ReviseRestockPolicyRequest(
                    campaignId,
                    shopId,
                    request.ExpectedVersion,
                    request.TargetOfferCount,
                    request.ToConstraints(),
                    request.ToWeights(),
                    CurrentUserId() ),
                cancellationToken );
            return Ok( result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "wallets/{characterId:int}/adjustments" )]
    public async Task<ActionResult<WalletDto>> AdjustWallet(
        int campaignId,
        int characterId,
        [FromBody] AdjustWalletApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            WalletDto result = await _walletService.AdjustAsync(
                campaignId,
                characterId,
                request.OperationId,
                request.AmountCopper,
                request.Description,
                CurrentUserId(),
                cancellationToken );
            return Ok( result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "shops/{shopId:int}/catalog-offers" )]
    public async Task<ActionResult<ShopOfferDto>> CreateCatalogOffer(
        int campaignId,
        int shopId,
        [FromBody] CreateCatalogOfferApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            ShopOfferDto result = await _offerService.CreateCatalogOfferAsync(
                campaignId,
                shopId,
                request.ItemConfigurationId,
                request.Quantity,
                CurrentUserId(),
                cancellationToken );
            return Created(
                $"api/commerce-admin/campaigns/{campaignId}/shops/{shopId}/offers/{result.OfferKey}",
                result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "shops/{shopId:int}/stock-offers" )]
    public async Task<ActionResult<ShopOfferDto>> CreateStockOffer(
        int campaignId,
        int shopId,
        [FromBody] CreateStockOfferApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            ShopOfferDto result = await _offerService.CreateStockInstanceOfferAsync(
                campaignId,
                shopId,
                request.ItemInstanceKey,
                request.Quantity,
                request.UnitPriceCopper,
                CurrentUserId(),
                cancellationToken );
            return Created(
                $"api/commerce-admin/campaigns/{campaignId}/shops/{shopId}/offers/{result.OfferKey}",
                result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "settlements" )]
    public async Task<ActionResult<SettlementDto>> CreateSettlement(
        int campaignId,
        [FromBody] CreateSettlementApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            SettlementDto result = await _service.CreateSettlementAsync(
                new CreateSettlementRequest(
                    campaignId,
                    request.Name,
                    request.Level,
                    request.Region,
                    request.Traits,
                    CurrentUserId() ),
                cancellationToken );
            return Created( $"api/commerce-admin/campaigns/{campaignId}/settlements/{result.Id}", result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "settlements/{settlementId:int}/shops" )]
    public async Task<ActionResult<ShopDto>> CreateShop(
        int campaignId,
        int settlementId,
        [FromBody] CreateShopApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            ShopDto result = await _service.CreateShopAsync(
                new CreateShopRequest(
                    campaignId,
                    settlementId,
                    request.Name,
                    request.Specialization,
                    request.ShopLevel,
                    CurrentUserId() ),
                cancellationToken );
            return Created(
                $"api/commerce-admin/campaigns/{campaignId}/settlements/{settlementId}/shops/{result.Id}",
                result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "shops/{shopId:int}/pricing-policy" )]
    public async Task<ActionResult<ShopDto>> UpdatePricingPolicy(
        int campaignId,
        int shopId,
        [FromBody] UpdateShopPricingPolicyApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            ShopDto result = await _service.UpdatePricingPolicyAsync(
                new UpdateShopPricingPolicyRequest(
                    campaignId,
                    shopId,
                    request.CatalogPricePercent,
                    request.BuybackPricePercent,
                    CurrentUserId() ),
                cancellationToken );
            return Ok( result );
        }
        catch ( UnauthorizedAccessException )
        {
            return Forbid();
        }
        catch ( CommerceException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }
}

public sealed record CreateSettlementApiRequest(
    string Name,
    int Level,
    string Region,
    string Traits );

public sealed record CreateShopApiRequest(
    string Name,
    string Specialization,
    int ShopLevel );

public sealed record UpdateShopPricingPolicyApiRequest(
    int CatalogPricePercent,
    int BuybackPricePercent );

public sealed record CreateCatalogOfferApiRequest(
    int ItemConfigurationId,
    int Quantity );

public sealed record CreateStockOfferApiRequest(
    Guid ItemInstanceKey,
    int Quantity,
    long UnitPriceCopper );

public sealed record AdjustWalletApiRequest(
    Guid OperationId,
    long AmountCopper,
    string Description );

public sealed record CreateRestockPolicyApiRequest(
    string Name,
    int TargetOfferCount,
    int MinimumItemLevel,
    int MaximumItemLevel,
    long BudgetCopper,
    RestockItemRarity AllowedRarities,
    RestockItemAccess AllowedAccess,
    RestockItemCategory AllowedCategories,
    int ConsumableWeight,
    int PermanentWeight,
    int UniqueWeight )
{
    public RestockPolicyConstraints ToConstraints() => new RestockPolicyConstraints(
        MinimumItemLevel,
        MaximumItemLevel,
        BudgetCopper,
        AllowedRarities,
        AllowedAccess,
        AllowedCategories );

    public RestockSelectionWeights ToWeights() => new RestockSelectionWeights(
        ConsumableWeight,
        PermanentWeight,
        UniqueWeight );
}

public sealed record ReviseRestockPolicyApiRequest(
    int ExpectedVersion,
    int TargetOfferCount,
    int MinimumItemLevel,
    int MaximumItemLevel,
    long BudgetCopper,
    RestockItemRarity AllowedRarities,
    RestockItemAccess AllowedAccess,
    RestockItemCategory AllowedCategories,
    int ConsumableWeight,
    int PermanentWeight,
    int UniqueWeight )
{
    public RestockPolicyConstraints ToConstraints() => new RestockPolicyConstraints(
        MinimumItemLevel,
        MaximumItemLevel,
        BudgetCopper,
        AllowedRarities,
        AllowedAccess,
        AllowedCategories );

    public RestockSelectionWeights ToWeights() => new RestockSelectionWeights(
        ConsumableWeight,
        PermanentWeight,
        UniqueWeight );
}

public sealed record GenerateRestockRunApiRequest(
    int PolicyVersion,
    long Seed );
