using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CampaignManagement.Application.Exceptions;
using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/campaign-parties" )]
public sealed class CampaignPartiesController : AuthorizedController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CampaignPartiesController> _logger;

    public CampaignPartiesController(
        IMediator mediator,
        ILogger<CampaignPartiesController> logger )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet( "characters" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<CampaignCharacterReference> ), StatusCodes.Status200OK )]
    public async Task<ActionResult<IReadOnlyCollection<CampaignCharacterReference>>> GetCharacters()
    {
        try
        {
            return Ok( await _mediator.Send( new GetCampaignCharactersQuery( CurrentUserId() ) ) );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "read campaign character candidates" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "read campaign character candidates" );
        }
    }

    [HttpPost( "campaigns/{campaignId:int}" )]
    [ProducesResponseType( typeof( CampaignDto ), StatusCodes.Status200OK )]
    public async Task<ActionResult<CampaignDto>> Create(
        int campaignId,
        [FromBody] CreateCampaignPartyRequest request )
    {
        try
        {
            CampaignDto campaign = await _mediator.Send(
                new CreateCampaignPartyCommand( CurrentUserId(), campaignId, request.Name ) );
            return Ok( campaign );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidation( exception ) );
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return NotFound( MapError( exception.Message ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "create a campaign party" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "create a campaign party" );
        }
    }

    [HttpPost( "campaigns/{campaignId:int}/characters" )]
    [ProducesResponseType( typeof( CampaignDto ), StatusCodes.Status200OK )]
    public async Task<ActionResult<CampaignDto>> AssignCharacter(
        int campaignId,
        [FromBody] AssignCampaignPartyCharacterRequest request )
    {
        try
        {
            int userId = CurrentUserId();
            CampaignDto campaign = await _mediator.Send(
                new AssignCampaignPartyCharacterCommand(
                    userId,
                    campaignId,
                    request.CharacterId,
                    request.ControlledByUserId ?? userId ) );
            return Ok( campaign );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return NotFound( MapError( exception.Message ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "assign a campaign party character" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "assign a campaign party character" );
        }
    }

    private ObjectResult DatabaseUnavailable( Exception exception, string operation )
    {
        _logger.LogError( exception, "Failed to {Operation} in the database.", operation );
        return StatusCode(
            StatusCodes.Status503ServiceUnavailable,
            MapError( "Campaign data is temporarily unavailable." ) );
    }
}

public sealed record CreateCampaignPartyRequest( string Name );

public sealed record AssignCampaignPartyCharacterRequest(
    int CharacterId,
    int? ControlledByUserId );
