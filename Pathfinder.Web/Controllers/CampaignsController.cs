using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

[Route( "api/campaigns" )]
public sealed class CampaignsController : AuthorizedController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CampaignsController> _logger;

    public CampaignsController( IMediator mediator, ILogger<CampaignsController> logger )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<CampaignDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status503ServiceUnavailable )]
    public async Task<ActionResult<IReadOnlyCollection<CampaignDto>>> Get()
    {
        try
        {
            int userId = GetCurrentUserId();
            IReadOnlyCollection<CampaignDto> campaigns = await _mediator.Send( new GetCampaignsQuery( userId ) );
            return Ok( campaigns );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return BadRequest( MapErrorMessage( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "read campaigns" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "read campaigns" );
        }
    }

    [HttpPost]
    [ProducesResponseType( typeof( CampaignDto ), StatusCodes.Status201Created )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status503ServiceUnavailable )]
    public async Task<ActionResult<CampaignDto>> Create( [FromBody] CreateCampaignRequest request )
    {
        try
        {
            int userId = GetCurrentUserId();
            CampaignDto campaign = await _mediator.Send( new CreateCampaignCommand( userId, request.Name ) );
            return CreatedAtAction( nameof( Get ), campaign );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidationErrors( exception ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapErrorMessage( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "create a campaign" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "create a campaign" );
        }
    }

    [HttpPost( "{campaignId:int}/archive" )]
    [ProducesResponseType( typeof( CampaignDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status404NotFound )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status503ServiceUnavailable )]
    public async Task<ActionResult<CampaignDto>> Archive( int campaignId )
    {
        try
        {
            int userId = GetCurrentUserId();
            CampaignDto campaign = await _mediator.Send( new ArchiveCampaignCommand( userId, campaignId ) );
            return Ok( campaign );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( CampaignManagementApplicationException exception )
        {
            return NotFound( MapErrorMessage( exception.Message ) );
        }
        catch ( CampaignManagementException exception )
        {
            return BadRequest( MapErrorMessage( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "archive a campaign" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "archive a campaign" );
        }
    }

    private ObjectResult DatabaseUnavailable( Exception exception, string operation )
    {
        _logger.LogError( exception, "Failed to {Operation} in the database.", operation );
        return StatusCode(
            StatusCodes.Status503ServiceUnavailable,
            MapErrorMessage( "Campaign data is temporarily unavailable." ) );
    }

    private static IReadOnlyCollection<string> MapErrorMessage( string message ) => [ message ];

    private static IReadOnlyCollection<string> MapValidationErrors( ValidationException exception ) => exception.Errors
        .Select( error => error.ErrorMessage )
        .Distinct()
        .ToArray();

    private int GetCurrentUserId()
    {
        string? rawUserId = User.FindFirstValue( ClaimTypes.NameIdentifier );
        if ( !Int32.TryParse( rawUserId, out int userId ) )
        {
            throw new InvalidOperationException( "Current user identifier claim is missing." );
        }

        return userId;
    }
}

public sealed record CreateCampaignRequest( string Name );
