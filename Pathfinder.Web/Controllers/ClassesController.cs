using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ClericDoctrines;
using Pathfinder.CharacterManagement.Application.UseCases.ClericDomains;
using Pathfinder.CharacterManagement.Application.UseCases.ClericSpells;
using Pathfinder.CharacterManagement.Application.UseCases.CharacterClasses;
using Pathfinder.CharacterManagement.Application.UseCases.Deities;
using Pathfinder.CharacterManagement.Application.UseCases.RogueRackets;
using Pathfinder.CharacterManagement.Application.UseCases.HuntersEdges;
using Pathfinder.CharacterManagement.Application.UseCases.DruidicOrders;
using Pathfinder.CharacterManagement.Application.UseCases.BardMuses;
using Pathfinder.CharacterManagement.Application.UseCases.WitchPatrons;
using Pathfinder.CharacterManagement.Application.UseCases.ArcaneSchools;
using Pathfinder.CharacterManagement.Application.UseCases.ArcaneTheses;
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

    [HttpGet( "rogue/rackets" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<RogueRacketDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<RogueRacketDto>>> GetRogueRackets()
    {
        IReadOnlyCollection<RogueRacketDto> rackets = await _mediator.Send( new GetRogueRacketsCommand() );
        return Ok( rackets );
    }

    [HttpGet( "ranger/hunters-edges" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<HuntersEdgeDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<HuntersEdgeDto>>> GetHuntersEdges()
    {
        IReadOnlyCollection<HuntersEdgeDto> huntersEdges = await _mediator.Send(
            new GetHuntersEdgesCommand() );
        return Ok( huntersEdges );
    }

    [HttpGet( "druid/orders" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<DruidicOrderDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<DruidicOrderDto>>> GetDruidicOrders()
    {
        IReadOnlyCollection<DruidicOrderDto> druidicOrders = await _mediator.Send(
            new GetDruidicOrdersCommand() );
        return Ok( druidicOrders );
    }

    [HttpGet( "bard/muses" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<BardMuseDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<BardMuseDto>>> GetBardMuses()
    {
        IReadOnlyCollection<BardMuseDto> bardMuses = await _mediator.Send(
            new GetBardMusesCommand() );
        return Ok( bardMuses );
    }

    [HttpGet( "witch/patrons" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<WitchPatronDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<WitchPatronDto>>> GetWitchPatrons()
    {
        IReadOnlyCollection<WitchPatronDto> patrons = await _mediator.Send(
            new GetWitchPatronsCommand() );
        return Ok( patrons );
    }

    [HttpGet( "wizard/arcane-schools" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<ArcaneSchoolDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<ArcaneSchoolDto>>> GetArcaneSchools()
    {
        IReadOnlyCollection<ArcaneSchoolDto> schools = await _mediator.Send(
            new GetArcaneSchoolsCommand() );
        return Ok( schools );
    }

    [HttpGet( "wizard/arcane-theses" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<ArcaneThesisDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<ArcaneThesisDto>>> GetArcaneTheses()
    {
        IReadOnlyCollection<ArcaneThesisDto> theses = await _mediator.Send(
            new GetArcaneThesesCommand() );
        return Ok( theses );
    }

    [HttpGet( "cleric/doctrines" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<ClericDoctrineDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<ClericDoctrineDto>>> GetClericDoctrines()
    {
        IReadOnlyCollection<ClericDoctrineDto> doctrines = await _mediator.Send(
            new GetClericDoctrinesCommand() );
        return Ok( doctrines );
    }

    [HttpGet( "cleric/deities" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<DeityDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<DeityDto>>> GetDeities()
    {
        IReadOnlyCollection<DeityDto> deities = await _mediator.Send( new GetDeitiesCommand() );
        return Ok( deities );
    }

    [HttpGet( "cleric/domains" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<ClericDomainDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<ClericDomainDto>>> GetClericDomains()
    {
        IReadOnlyCollection<ClericDomainDto> domains = await _mediator.Send(
            new GetClericDomainsCommand() );
        return Ok( domains );
    }

    [HttpGet( "cleric/spells" )]
    [ProducesResponseType( typeof( IReadOnlyCollection<SpellDefinitionDto> ), StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    public async Task<ActionResult<IReadOnlyCollection<SpellDefinitionDto>>> GetClericSpells()
    {
        IReadOnlyCollection<SpellDefinitionDto> spells = await _mediator.Send(
            new GetClericSpellsCommand() );
        return Ok( spells );
    }
}
