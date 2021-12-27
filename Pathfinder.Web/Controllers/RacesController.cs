using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.UseCases.Races;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class RacesController : AuthorizedController
    {
        private readonly IMediator _mediator;

        public RacesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<RaceDto>> Races()
        {
            return Ok(await _mediator.Send(new GetRacesCommand()));
        }
    }
}