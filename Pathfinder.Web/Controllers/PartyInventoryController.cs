using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pathfinder.Inventory.Application.Transfers;
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
