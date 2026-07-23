using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Application.Offers;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/commerce-admin/campaigns/{campaignId:int}" )]
public sealed class CommerceAdminController : AuthorizedController
{
    private readonly ShopAdministrationService _service;
    private readonly ShopOfferAdministrationService _offerService;

    public CommerceAdminController(
        ShopAdministrationService service,
        ShopOfferAdministrationService offerService )
    {
        _service = service;
        _offerService = offerService;
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

public sealed record CreateCatalogOfferApiRequest(
    int ItemConfigurationId,
    int Quantity,
    long UnitPriceCopper );

public sealed record CreateStockOfferApiRequest(
    Guid ItemInstanceKey,
    int Quantity,
    long UnitPriceCopper );
