using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Feats;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/feats" )]
public sealed class FeatsController : AuthorizedController
{
    private readonly IMediator _mediator;

    public FeatsController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<FeatDefinitionDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<FeatDefinitionDto>>> Get(
        [FromQuery] FeatCategory category,
        [FromQuery] int level,
        [FromQuery] string? requiredTrait = null )
    {
        IReadOnlyCollection<FeatDefinitionDto> feats = await _mediator.Send(
            new GetFeatOptionsCommand( category, level, requiredTrait ) );
        return Ok( feats );
    }
}
