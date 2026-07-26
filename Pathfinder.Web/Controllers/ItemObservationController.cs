using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;
using Pathfinder.Web.Integration;

namespace Pathfinder.Web.Controllers;

[Route( "api/campaigns/{campaignId:int}/items" )]
public sealed class ItemObservationController : AuthorizedController
{
    private readonly ItemObservationService _observationService;

    public ItemObservationController( ItemObservationService observationService )
    {
        _observationService = observationService;
    }

    [HttpGet( "{instanceKey:guid}" )]
    public async Task<ActionResult<VisibleItemDto>> Get(
        int campaignId,
        Guid instanceKey,
        [FromQuery] int? observerCharacterId,
        CancellationToken cancellationToken )
    {
        try
        {
            VisibleItemDto result = await _observationService.GetVisibleAsync(
                campaignId,
                instanceKey,
                CurrentUserId(),
                observerCharacterId,
                cancellationToken );
            return Ok( result );
        }
        catch ( ItemObservationAccessDeniedException )
        {
            return Forbid();
        }
        catch ( InventoryException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
    }
}