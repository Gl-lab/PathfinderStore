using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.CharacterClasses;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/classes" )]
public sealed class ClassesController : AuthorizedController
{
    private readonly IMediator _mediator;

    public ClassesController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<CharacterClassDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<CharacterClassDto>>> Get()
    {
        IReadOnlyCollection<CharacterClassDto> characterClasses = await _mediator.Send( new GetCharacterClassesCommand() );
        return Ok( characterClasses );
    }
}
