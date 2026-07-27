using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Web.Integration;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Transactions;

namespace Pathfinder.Web.Controllers;

[Route( "api/commerce/campaigns/{campaignId:int}" )]
public sealed class CommerceController : AuthorizedController
{
    private readonly PurchaseReservationService _reservationService;
    private readonly CommerceReadProjectionService _readProjectionService;

    public CommerceController(
        PurchaseReservationService reservationService,
        CommerceReadProjectionService readProjectionService )
    {
        _reservationService = reservationService;
        _readProjectionService = readProjectionService;
    }

    [HttpGet( "wallets/{characterId:int}" )]
    public async Task<ActionResult<WalletDto>> GetWallet(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken )
    {
        try
        {
            WalletDto wallet = await _readProjectionService.GetWalletAsync(
                campaignId,
                characterId,
                CurrentUserId(),
                cancellationToken );
            return Ok( wallet );
        }
        catch ( CommerceReadAccessDeniedException )
        {
            return Forbid();
        }
    }

    [HttpGet( "settlements" )]
    public async Task<ActionResult<IReadOnlyCollection<SettlementDto>>> GetSettlements(
        int campaignId,
        CancellationToken cancellationToken )
    {
        try
        {
            IReadOnlyCollection<SettlementDto> settlements =
                await _readProjectionService.GetSettlementsAsync(
                    campaignId,
                    CurrentUserId(),
                    cancellationToken );
            return Ok( settlements );
        }
        catch ( CommerceReadAccessDeniedException )
        {
            return Forbid();
        }
    }

    [HttpGet( "shops/{shopId:int}/offers" )]
    public async Task<ActionResult<IReadOnlyCollection<CommerceShopOfferDto>>> GetOffers(
        int campaignId,
        int shopId,
        [FromQuery] CommerceOfferStatusFilter status = CommerceOfferStatusFilter.Active,
        CancellationToken cancellationToken = default )
    {
        try
        {
            IReadOnlyCollection<CommerceShopOfferDto> offers =
                await _readProjectionService.GetOffersAsync(
                    campaignId,
                    shopId,
                    CurrentUserId(),
                    status,
                    cancellationToken );
            return Ok( offers );
        }
        catch ( CommerceReadAccessDeniedException )
        {
            return Forbid();
        }
        catch ( CommerceReadNotFoundException )
        {
            return NotFound();
        }
    }

    [HttpGet( "purchase-reservations" )]
    public async Task<ActionResult<IReadOnlyCollection<CommercePurchaseReservationDto>>>
        GetPurchaseReservations(
            int campaignId,
            [FromQuery] int buyerCharacterId,
            [FromQuery] PurchaseReservationStatus? status,
            CancellationToken cancellationToken )
    {
        try
        {
            IReadOnlyCollection<CommercePurchaseReservationDto> reservations =
                await _readProjectionService.GetPurchaseReservationsAsync(
                    campaignId,
                    buyerCharacterId,
                    CurrentUserId(),
                    status,
                    cancellationToken );
            return Ok( reservations );
        }
        catch ( CommerceReadAccessDeniedException )
        {
            return Forbid();
        }
    }

    [HttpGet( "shops/{shopId:int}/sell-quote" )]
    public async Task<ActionResult<CommerceSellQuoteDto>> GetSellQuote(
        int campaignId,
        int shopId,
        [FromQuery] int sellerCharacterId,
        [FromQuery] Guid itemInstanceKey,
        CancellationToken cancellationToken )
    {
        try
        {
            CommerceSellQuoteDto quote = await _readProjectionService.GetSellQuoteAsync(
                campaignId,
                shopId,
                sellerCharacterId,
                itemInstanceKey,
                CurrentUserId(),
                cancellationToken );
            return Ok( quote );
        }
        catch ( CommerceReadAccessDeniedException )
        {
            return Forbid();
        }
        catch ( CommerceReadNotFoundException )
        {
            return NotFound();
        }
    }

    [HttpPost( "purchase-reservations" )]
    public async Task<ActionResult<PurchaseReservationDto>> ReservePurchase(
        int campaignId,
        [FromBody] ReservePurchaseApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            PurchaseReservationDto result = await _reservationService.ReserveAsync(
                campaignId,
                request.OperationId,
                request.OfferKey,
                request.BuyerCharacterId,
                request.Quantity,
                CurrentUserId(),
                cancellationToken );
            return Created(
                $"api/commerce/campaigns/{campaignId}/purchase-reservations/{result.ReservationKey}",
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

    [HttpPost( "purchase-reservations/{reservationKey:guid}/cancel" )]
    public async Task<ActionResult<PurchaseReservationDto>> CancelPurchaseReservation(
        int campaignId,
        Guid reservationKey,
        [FromBody] CancelPurchaseReservationApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            PurchaseReservationDto result = await _reservationService.CancelAsync(
                campaignId,
                reservationKey,
                request.OperationId,
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

    [HttpPost( "purchase-reservations/{reservationKey:guid}/complete" )]
    public async Task<ActionResult<PurchaseReservationDto>> CompletePurchase(
        int campaignId,
        Guid reservationKey,
        [FromBody] CompletePurchaseApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            PurchaseReservationDto result = await _reservationService.CompleteAsync(
                campaignId,
                reservationKey,
                request.OperationId,
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
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }

    [HttpPost( "shops/{shopId:int}/sales" )]
    public async Task<ActionResult<ShopSaleDto>> SellItem(
        int campaignId,
        int shopId,
        [FromBody] SellItemApiRequest request,
        CancellationToken cancellationToken )
    {
        try
        {
            ShopSaleDto result = await _reservationService.SellAsync(
                campaignId,
                shopId,
                request.SellerCharacterId,
                request.ItemInstanceKey,
                request.OperationId,
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
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }
}

public sealed record ReservePurchaseApiRequest(
    Guid OperationId,
    Guid OfferKey,
    int BuyerCharacterId,
    int Quantity );

public sealed record CancelPurchaseReservationApiRequest( Guid OperationId );

public sealed record CompletePurchaseApiRequest( Guid OperationId );

public sealed record SellItemApiRequest(
    Guid OperationId,
    int SellerCharacterId,
    Guid ItemInstanceKey );
