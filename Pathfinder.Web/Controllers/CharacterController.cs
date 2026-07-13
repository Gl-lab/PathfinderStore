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
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/character" )]
public sealed class CharacterController : AuthorizedController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CharacterController> _logger;

    public CharacterController(
        IMediator mediator,
        ILogger<CharacterController> logger )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<CharacterDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status503ServiceUnavailable )]
    public async Task<ActionResult<IReadOnlyCollection<CharacterDto>>> Get()
    {
        try
        {
            int userId = GetCurrentUserId();
            IReadOnlyCollection<CharacterDto> characters = await _mediator.Send( new GetCharactersCommand( userId ) );
            return Ok( characters );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidationErrors( exception ) );
        }
        catch ( CharacterManagementException exception )
        {
            return BadRequest( MapErrorMessage( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            _logger.LogError( exception, "Failed to read characters from the database." );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
        catch ( PostgresException exception )
        {
            _logger.LogError( exception, "PostgreSQL failed while reading characters." );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
    }

    [HttpGet( "{characterId:int}" )]
    [ProducesResponseType( typeof( CharacterDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status404NotFound )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status503ServiceUnavailable )]
    public async Task<ActionResult<CharacterDto>> GetById( int characterId )
    {
        try
        {
            int userId = GetCurrentUserId();
            CharacterDto character = await _mediator.Send( new GetCharacterByIdCommand( userId, characterId ) );
            return Ok( character );
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidationErrors( exception ) );
        }
        catch ( CharacterManagementException exception )
        {
            return NotFound( MapErrorMessage( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            _logger.LogError(
                exception,
                "Failed to read character {CharacterId} from the database.",
                characterId );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
        catch ( PostgresException exception )
        {
            _logger.LogError(
                exception,
                "PostgreSQL failed while reading character {CharacterId}.",
                characterId );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
    }

    [HttpPost]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status503ServiceUnavailable )]
    public async Task<ActionResult> Create( [FromBody] CreateCharacterRequestDto character )
    {
        try
        {
            int userId = GetCurrentUserId();
            await _mediator.Send( new CreateCharacterCommand( userId, character ) );
            return Ok();
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidationErrors( exception ) );
        }
        catch ( CharacterManagementException exception )
        {
            return BadRequest( MapErrorMessage( exception.Message ) );
        }
        catch ( Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException exception )
        {
            return BadRequest( MapErrorMessage( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            _logger.LogError( exception, "Failed to create a character in the database." );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
        catch ( PostgresException exception )
        {
            _logger.LogError( exception, "PostgreSQL failed while creating a character." );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
    }

    [HttpDelete( "{characterId:int}" )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status400BadRequest )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status404NotFound )]
    [ProducesResponseType( typeof( IReadOnlyCollection<string> ), StatusCodes.Status503ServiceUnavailable )]
    public async Task<ActionResult> Delete( int characterId )
    {
        try
        {
            int userId = GetCurrentUserId();
            await _mediator.Send( new DeleteCharacterCommand( userId, characterId ) );
            return Ok();
        }
        catch ( InvalidOperationException )
        {
            return Unauthorized();
        }
        catch ( ValidationException exception )
        {
            return BadRequest( MapValidationErrors( exception ) );
        }
        catch ( CharacterManagementException exception )
        {
            return NotFound( MapErrorMessage( exception.Message ) );
        }
        catch ( DbUpdateException exception )
        {
            _logger.LogError(
                exception,
                "Failed to delete character {CharacterId} from the database.",
                characterId );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
        catch ( PostgresException exception )
        {
            _logger.LogError(
                exception,
                "PostgreSQL failed while deleting character {CharacterId}.",
                characterId );
            return StatusCode( StatusCodes.Status503ServiceUnavailable, MapErrorMessage( "Character data is temporarily unavailable." ) );
        }
    }

    [HttpGet]
    [Route( "items" )]
    public ActionResult Items() => Ok();

    [HttpDelete]
    [Route( "items/drop" )]
    public ActionResult ItemDrop() => Ok();

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
