using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Ancestries;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/ancestries" )]
public sealed class AncestriesController : AuthorizedController
{
    private readonly IMediator _mediator;

    public AncestriesController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<AncestryDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<AncestryDto>>> Get()
    {
        IReadOnlyCollection<AncestryDto> ancestries = await _mediator.Send( new GetAncestriesCommand() );
        return Ok( ancestries );
    }
}