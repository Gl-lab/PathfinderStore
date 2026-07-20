using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Languages;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/languages" )]
public sealed class LanguagesController : AuthorizedController
{
    private readonly IMediator _mediator;

    public LanguagesController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<LanguageDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<LanguageDto>>> Get()
    {
        IReadOnlyCollection<LanguageDto> languages = await _mediator.Send( new GetLanguagesCommand() );
        return Ok( languages );
    }

    [HttpGet( "options" )]
    [ProducesResponseType( typeof( LanguageSelectionOptionsDto ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<LanguageSelectionOptionsDto>> GetOptions(
        [FromQuery] AncestryType ancestryType,
        [FromQuery] int intelligenceScore )
    {
        LanguageSelectionOptionsDto options = await _mediator.Send(
            new GetLanguageSelectionOptionsCommand( ancestryType, intelligenceScore ) );
        return Ok( options );
    }
}
