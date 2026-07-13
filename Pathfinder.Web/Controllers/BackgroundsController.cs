using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Backgrounds;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/backgrounds" )]
public sealed class BackgroundsController : AuthorizedController
{
    private readonly IMediator _mediator;

    public BackgroundsController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<BackgroundDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<BackgroundDto>>> Get()
    {
        IReadOnlyCollection<BackgroundDto> backgrounds = await _mediator.Send( new GetBackgroundsCommand() );
        return Ok( backgrounds );
    }
}
