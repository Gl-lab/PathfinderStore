using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Spells;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/spells" )]
public sealed class SpellsController : AuthorizedController
{
    private readonly IMediator _mediator;

    public SpellsController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<SpellDefinitionDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<SpellDefinitionDto>>> Get(
        [FromQuery] SpellTradition tradition,
        [FromQuery] int rank,
        [FromQuery] SpellKind kind )
    {
        IReadOnlyCollection<SpellDefinitionDto> spells = await _mediator.Send(
            new GetSpellOptionsCommand( tradition, rank, kind ) );
        return Ok( spells );
    }
}
