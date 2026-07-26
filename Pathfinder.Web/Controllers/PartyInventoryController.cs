using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pathfinder.Inventory.Application.Transfers;
using Pathfinder.Inventory.Application.Storage;
using Pathfinder.Inventory.Application.Administration;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Web.Integration;

namespace Pathfinder.Web.Controllers;

[Route( "api/campaigns/{campaignId:int}/inventory" )]
public sealed class PartyInventoryController : AuthorizedController
{
    private readonly IMediator _mediator;
    private readonly CharacterInventoryProjectionService _projectionService;
    private readonly InventoryOperationsProjectionService _operationsProjectionService;
    private readonly ILogger<PartyInventoryController> _logger;

    public PartyInventoryController(
        IMediator mediator,
        CharacterInventoryProjectionService projectionService,
        InventoryOperationsProjectionService operationsProjectionService,
        ILogger<PartyInventoryController> logger )
    {
        _mediator = mediator;
        _projectionService = projectionService;
        _operationsProjectionService = operationsProjectionService;
        _logger = logger;
    }

    [HttpGet( "gifts" )]
    public async Task<ActionResult<IReadOnlyCollection<PartyGiftProjectionDto>>> GetGifts(
        int campaignId,
        [FromQuery] int characterId,
        [FromQuery] PartyGiftRole role,
        [FromQuery] PartyGiftStatus status = PartyGiftStatus.Pending,
        CancellationToken cancellationToken = default )
    {
        try
        {
            IReadOnlyCollection<PartyGiftProjectionDto> gifts =
                await _operationsProjectionService.GetGiftsAsync(
                    campaignId,
                    characterId,
                    role,
                    status,
                    CurrentUserId(),
                    cancellationToken );
            return Ok( gifts );
        }
        catch ( InventoryOperationsAccessDeniedException )
        {
            return Forbid();
        }
    }

    [HttpGet( "exchanges" )]
    public async Task<ActionResult<IReadOnlyCollection<PartyExchangeProjectionDto>>> GetExchanges(
        int campaignId,
        [FromQuery] int participantCharacterId,
        [FromQuery] PartyExchangeStatus status = PartyExchangeStatus.Pending,
        CancellationToken cancellationToken = default )
    {
        try
        {
            IReadOnlyCollection<PartyExchangeProjectionDto> exchanges =
                await _operationsProjectionService.GetExchangesAsync(
                    campaignId,
                    participantCharacterId,
                    status,
                    CurrentUserId(),
                    cancellationToken );
            return Ok( exchanges );
        }
        catch ( InventoryOperationsAccessDeniedException )
        {
            return Forbid();
        }
    }

    [HttpGet( "party-storage" )]
    public async Task<ActionResult<PartyStorageProjectionDto>> GetPartyStorage(
        int campaignId,
        CancellationToken cancellationToken )
    {
        try
        {
            PartyStorageProjectionDto storage =
                await _operationsProjectionService.GetPartyStorageAsync(
                    campaignId,
                    CurrentUserId(),
                    cancellationToken );
            return Ok( storage );
        }
        catch ( InventoryOperationsAccessDeniedException )
        {
            return Forbid();
        }
    }

    [HttpGet( "characters/{characterId:int}" )]
    [ProducesResponseType( typeof( CharacterInventoryDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status403Forbidden )]
    [ProducesResponseType( StatusCodes.Status404NotFound )]
    public async Task<ActionResult<CharacterInventoryDto>> GetCharacterInventory(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken )
    {
        try
        {
            CharacterInventoryDto inventory = await _projectionService.GetAsync(
                campaignId,
                characterId,
                CurrentUserId(),
                cancellationToken );
            return Ok( inventory );
        }
        catch ( CharacterInventoryAccessDeniedException )
        {
            return Forbid();
        }
        catch ( CharacterInventoryNotFoundException )
        {
            return NotFound();
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

    [HttpPost( "force-move" )]
    [ProducesResponseType( typeof( ForcedInventoryMoveDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    public async Task<ActionResult<ForcedInventoryMoveDto>> ForceMove(
        int campaignId,
        [FromBody] ForceMoveInventoryItemRequest request )
    {
        try
        {
            ForcedInventoryMoveDto result = await _mediator.Send(
                new ForceMoveInventoryItemCommand(
                    CurrentUserId(),
                    campaignId,
                    request.ItemInstanceKey,
                    request.DestinationContainerKey,
                    request.ExpectedItemVersion,
                    request.OperationId,
                    request.Reason ) );
            return Ok( result );
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

    [HttpPost( "correct-transfer-restriction" )]
    public async Task<ActionResult<CorrectedItemTransferRestrictionDto>>
        CorrectTransferRestriction(
            int campaignId,
            [FromBody] CorrectItemTransferRestrictionRequest request )
    {
        try
        {
            CorrectedItemTransferRestrictionDto result = await _mediator.Send(
                new CorrectItemTransferRestrictionCommand(
                    CurrentUserId(),
                    campaignId,
                    request.ItemInstanceKey,
                    request.IsTransferRestricted,
                    request.ExpectedItemVersion,
                    request.OperationId,
                    request.Reason ) );
            return Ok( result );
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

public sealed record ForceMoveInventoryItemRequest(
    Guid ItemInstanceKey,
    Guid DestinationContainerKey,
    int ExpectedItemVersion,
    Guid OperationId,
    string Reason );

public sealed record CorrectItemTransferRestrictionRequest(
    Guid ItemInstanceKey,
    bool IsTransferRestricted,
    int ExpectedItemVersion,
    Guid OperationId,
    string Reason );