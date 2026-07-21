using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Equipment;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

[Route( "api/equipment" )]
public sealed class EquipmentController : AuthorizedController
{
    private readonly IMediator _mediator;

    public EquipmentController( IMediator mediator )
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType( typeof( IReadOnlyCollection<EquipmentDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<EquipmentDto>>> Get()
    {
        IReadOnlyCollection<EquipmentDto> equipment = await _mediator.Send( new GetEquipmentCommand() );
        return Ok( equipment );
    }

    [HttpGet( "class-kits" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<ClassKitDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<ClassKitDto>>> GetClassKits()
    {
        IReadOnlyCollection<ClassKitDto> kits = await _mediator.Send( new GetClassKitsCommand() );
        return Ok( kits );
    }
}
