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
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/campaigns/{campaignId:int}/characters" )]
public sealed class CampaignCharactersController : AuthorizedController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CampaignCharactersController> _logger;

    public CampaignCharactersController(
        IMediator mediator,
        ILogger<CampaignCharactersController> logger )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet( "{characterId:int}" )]
    [ProducesResponseType( typeof( CampaignCharacterDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status404NotFound )]
    public async Task<ActionResult<CampaignCharacterDto>> GetById(
        int campaignId,
        int characterId )
    {
        try
        {
            CampaignCharacterDto character = await _mediator.Send(
                new GetCampaignCharacterByIdCommand(
                    CurrentUserId(),
                    campaignId,
                    characterId ) );
            return Ok( character );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( CharacterManagementException exception )
        {
            return NotFound( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "read a campaign character" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "read a campaign character" );
        }
    }

    [HttpPost( "{characterId:int}/hit-points" )]
    [ProducesResponseType( typeof( CharacterHitPointStateDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status404NotFound )]
    public async Task<ActionResult<CharacterHitPointStateDto>> ChangeHitPoints(
        int campaignId,
        int characterId,
        [FromBody] ChangeHitPointsRequestDto request )
    {
        try
        {
            CharacterHitPointStateDto state = await _mediator.Send(
                new ChangeHitPointsCommand(
                    CurrentUserId(),
                    campaignId,
                    characterId,
                    request.Operation,
                    request.Amount ) );
            return Ok( state );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidation( exception ) );
        }
        catch ( CharacterManagementException exception )
        {
            return NotFound( MapError( exception.Message ) );
        }
        catch ( Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException exception )
        {
            return BadRequest( MapError( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            return DatabaseUnavailable( exception, "change campaign character hit points" );
        }
        catch ( PostgresException exception )
        {
            return DatabaseUnavailable( exception, "change campaign character hit points" );
        }
    }

    private ObjectResult DatabaseUnavailable( Exception exception, string operation )
    {
        _logger.LogError( exception, "Failed to {Operation} in the database.", operation );
        return StatusCode(
            StatusCodes.Status503ServiceUnavailable,
            MapError( "Character data is temporarily unavailable." ) );
    }
}
