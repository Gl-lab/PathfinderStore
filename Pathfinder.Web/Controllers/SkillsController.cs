using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Skills;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/skills" )]
public sealed class SkillsController : AuthorizedController
{
    private readonly IMediator _mediator;

    public SkillsController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<SkillDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<SkillDto>>> Get()
    {
        IReadOnlyCollection<SkillDto> skills = await _mediator.Send( new GetSkillsCommand() );
        return Ok( skills );
    }
}
