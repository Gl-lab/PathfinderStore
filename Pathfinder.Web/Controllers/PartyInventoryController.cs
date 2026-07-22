using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Application.Storage;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/campaigns/{campaignId:int}/inventory" )]
public sealed class PartyInventoryController : AuthorizedController
{
    private readonly IMediator _mediator;
    private readonly ILogger<PartyInventoryController> _logger;

    public PartyInventoryController(
        IMediator mediator,
        ILogger<PartyInventoryController> logger )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost( "gifts" )]
    [ProducesResponseType( typeof( PartyGiftDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    public async Task<ActionResult<PartyGiftDto>> CreateGift(
        int campaignId,
        [FromBody] CreatePartyGiftRequest request )
    {
        try
        {
            PartyGiftDto gift = await _mediator.Send( new CreatePartyGiftCommand(
                CurrentUserId(),
                campaignId,
                request.GiftKey,
                request.SourceCharacterId,
                request.DestinationCharacterId,
                request.ItemInstanceKey,
                request.ExpectedItemVersion ) );
            return Ok( gift );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception );
        }
    }

    [HttpPost( "gifts/{giftKey:guid}/accept" )]
    [ProducesResponseType( typeof( PartyGiftDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    public async Task<ActionResult<PartyGiftDto>> AcceptGift(
        int campaignId,
        Guid giftKey,
        [FromBody] AcceptPartyGiftRequest request )
    {
        try
        {
            PartyGiftDto gift = await _mediator.Send( new AcceptPartyGiftCommand(
                CurrentUserId(),
                campaignId,
                giftKey,
                request.OperationId ) );
            return Ok( gift );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception );
        }
    }

    [HttpPost( "exchanges" )]
    [ProducesResponseType( typeof( PartyExchangeDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    public async Task<ActionResult<PartyExchangeDto>> CreateExchange(
        int campaignId,
        [FromBody] CreatePartyExchangeRequest request )
    {
        try
        {
            PartyExchangeDto exchange = await _mediator.Send( new CreatePartyExchangeCommand(
                CurrentUserId(),
                campaignId,
                request.ExchangeKey,
                request.InitiatorCharacterId,
                request.CounterpartyCharacterId,
                request.Lines
                    .Select( line => new CreatePartyExchangeLine(
                        line.FromCharacterId,
                        line.ItemInstanceKey,
                        line.ExpectedItemVersion,
                        line.ReservationOperationId ) )
                    .ToArray() ) );
            return Ok( exchange );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception );
        }
    }

    [HttpPost( "exchanges/{exchangeKey:guid}/complete" )]
    [ProducesResponseType( typeof( PartyExchangeDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    public async Task<ActionResult<PartyExchangeDto>> CompleteExchange(
        int campaignId,
        Guid exchangeKey,
        [FromBody] FinalizePartyExchangeRequest request )
    {
        return await FinalizeExchange( userId => new CompletePartyExchangeCommand(
            userId,
            campaignId,
            exchangeKey,
            request.OperationId ) );
    }

    [HttpPost( "exchanges/{exchangeKey:guid}/cancel" )]
    [ProducesResponseType( typeof( PartyExchangeDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    public async Task<ActionResult<PartyExchangeDto>> CancelExchange(
        int campaignId,
        Guid exchangeKey,
        [FromBody] FinalizePartyExchangeRequest request )
    {
        return await FinalizeExchange( userId => new CancelPartyExchangeCommand(
            userId,
            campaignId,
            exchangeKey,
            request.OperationId ) );
    }

    private async Task<ActionResult<PartyExchangeDto>> FinalizeExchange(
        Func<int, IRequest<PartyExchangeDto>> commandFactory )
    {
        try
        {
            PartyExchangeDto exchange = await _mediator.Send( commandFactory( CurrentUserId() ) );
            return Ok( exchange );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception );
        }
    }

    [HttpPost( "party-storage/deposit" )]
    [ProducesResponseType( typeof( PartyStorageItemDto ), StatusCodes.Status200OK )]
    public async Task<ActionResult<PartyStorageItemDto>> DepositPartyStorage(
        int campaignId,
        [FromBody] PartyStorageTransferRequest request )
    {
        return await TransferPartyStorage( userId => new DepositPartyStorageCommand(
            userId,
            campaignId,
            request.CharacterId,
            request.ItemInstanceKey,
            request.ExpectedItemVersion,
            request.OperationId ) );
    }

    [HttpPost( "party-storage/withdraw" )]
    [ProducesResponseType( typeof( PartyStorageItemDto ), StatusCodes.Status200OK )]
    public async Task<ActionResult<PartyStorageItemDto>> WithdrawPartyStorage(
        int campaignId,
        [FromBody] PartyStorageTransferRequest request )
    {
        return await TransferPartyStorage( userId => new WithdrawPartyStorageCommand(
            userId,
            campaignId,
            request.CharacterId,
            request.ItemInstanceKey,
            request.ExpectedItemVersion,
            request.OperationId ) );
    }

    private async Task<ActionResult<PartyStorageItemDto>> TransferPartyStorage(
        Func<int, IRequest<PartyStorageItemDto>> commandFactory )
    {
        try
        {
            PartyStorageItemDto item = await _mediator.Send( commandFactory( CurrentUserId() ) );
            return Ok( item );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception );
        }
    }

    private ObjectResult DatabaseUnavailable( Exception exception )
    {
        _logger.LogError( exception, "Failed to update party inventory." );
        return StatusCode(
            StatusCodes.Status503ServiceUnavailable,
            MapError( "Inventory data is temporarily unavailable." ) );
    }
}

public sealed record CreatePartyGiftRequest(
    Guid GiftKey,
    int SourceCharacterId,
    int DestinationCharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion );

public sealed record AcceptPartyGiftRequest( Guid OperationId );

public sealed record CreatePartyExchangeRequest(
    Guid ExchangeKey,
    int InitiatorCharacterId,
    int CounterpartyCharacterId,
    IReadOnlyCollection<CreatePartyExchangeLineRequest> Lines );

public sealed record CreatePartyExchangeLineRequest(
    int FromCharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion,
    Guid ReservationOperationId );

public sealed record FinalizePartyExchangeRequest( Guid OperationId );

public sealed record PartyStorageTransferRequest(
    int CharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion,
    Guid OperationId );
