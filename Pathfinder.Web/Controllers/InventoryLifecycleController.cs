using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Inventory.Application.Lifecycle;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/campaigns/{campaignId:int}/inventory/lifecycle" )]
public sealed class InventoryLifecycleController : AuthorizedController
{
    private readonly IMediator _mediator;

    public InventoryLifecycleController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpPost( "items/{itemInstanceKey:guid}/charges/consume" )]
    public Task<ActionResult<ItemLifecycleDto>> ConsumeCharges(
        int campaignId,
        Guid itemInstanceKey,
        [FromBody] QuantityLifecycleRequest request,
        CancellationToken cancellationToken )
    {
        return Execute(
            new ConsumeItemChargesCommand(
                CurrentUserId(),
                campaignId,
                itemInstanceKey,
                request.Quantity,
                request.ExpectedVersion,
                request.OperationId ),
            cancellationToken );
    }

    [HttpPost( "items/{itemInstanceKey:guid}/charges/recover" )]
    public Task<ActionResult<ItemLifecycleDto>> RecoverCharges(
        int campaignId,
        Guid itemInstanceKey,
        [FromBody] QuantityLifecycleRequest request,
        CancellationToken cancellationToken )
    {
        return Execute(
            new RecoverItemChargesCommand(
                CurrentUserId(),
                campaignId,
                itemInstanceKey,
                request.Quantity,
                request.ExpectedVersion,
                request.OperationId ),
            cancellationToken );
    }

    [HttpPost( "items/{itemInstanceKey:guid}/consume" )]
    public Task<ActionResult<ItemLifecycleDto>> ConsumeItem(
        int campaignId,
        Guid itemInstanceKey,
        [FromBody] QuantityLifecycleRequest request,
        CancellationToken cancellationToken )
    {
        return Execute(
            new ConsumeInventoryItemCommand(
                CurrentUserId(),
                campaignId,
                itemInstanceKey,
                request.Quantity,
                request.ExpectedVersion,
                request.OperationId ),
            cancellationToken );
    }

    [HttpPost( "items/{itemInstanceKey:guid}/damage" )]
    public Task<ActionResult<ItemLifecycleDto>> DamageItem(
        int campaignId,
        Guid itemInstanceKey,
        [FromBody] QuantityLifecycleRequest request,
        CancellationToken cancellationToken )
    {
        return Execute(
            new DamageInventoryItemCommand(
                CurrentUserId(),
                campaignId,
                itemInstanceKey,
                request.Quantity,
                request.ExpectedVersion,
                request.OperationId ),
            cancellationToken );
    }

    [HttpPost( "items/{itemInstanceKey:guid}/repair" )]
    public Task<ActionResult<ItemLifecycleDto>> RepairItem(
        int campaignId,
        Guid itemInstanceKey,
        [FromBody] QuantityLifecycleRequest request,
        CancellationToken cancellationToken )
    {
        return Execute(
            new RepairInventoryItemCommand(
                CurrentUserId(),
                campaignId,
                itemInstanceKey,
                request.Quantity,
                request.ExpectedVersion,
                request.OperationId ),
            cancellationToken );
    }

    [HttpPost( "runes/{runeInstanceKey:guid}/attach" )]
    public Task<ActionResult<ItemLifecycleDto>> AttachRune(
        int campaignId,
        Guid runeInstanceKey,
        [FromBody] AttachRuneRequest request,
        CancellationToken cancellationToken )
    {
        return Execute(
            new AttachInventoryRuneCommand(
                CurrentUserId(),
                campaignId,
                runeInstanceKey,
                request.TargetInstanceKey,
                request.ExpectedRuneVersion,
                request.ExpectedTargetVersion,
                request.OperationId ),
            cancellationToken );
    }

    [HttpPost( "runes/{runeInstanceKey:guid}/transfer" )]
    public Task<ActionResult<ItemLifecycleDto>> TransferRune(
        int campaignId,
        Guid runeInstanceKey,
        [FromBody] TransferRuneRequest request,
        CancellationToken cancellationToken )
    {
        return Execute(
            new TransferInventoryRuneCommand(
                CurrentUserId(),
                campaignId,
                runeInstanceKey,
                request.SourceInstanceKey,
                request.DestinationInstanceKey,
                request.ExpectedRuneVersion,
                request.ExpectedSourceVersion,
                request.ExpectedDestinationVersion,
                request.OperationId ),
            cancellationToken );
    }

    private async Task<ActionResult<ItemLifecycleDto>> Execute(
        IRequest<ItemLifecycleDto> command,
        CancellationToken cancellationToken )
    {
        try
        {
            return Ok( await _mediator.Send( command, cancellationToken ) );
        }
        catch ( InventoryException exception )
        {
            return BadRequest( new[] { exception.Message } );
        }
    }
}

public sealed record QuantityLifecycleRequest(
    int Quantity,
    int ExpectedVersion,
    Guid OperationId );

public sealed record AttachRuneRequest(
    Guid TargetInstanceKey,
    int ExpectedRuneVersion,
    int ExpectedTargetVersion,
    Guid OperationId );

public sealed record TransferRuneRequest(
    Guid SourceInstanceKey,
    Guid DestinationInstanceKey,
    int ExpectedRuneVersion,
    int ExpectedSourceVersion,
    int ExpectedDestinationVersion,
    Guid OperationId );
