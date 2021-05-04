using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class RacesController: AuthorizedController
    {
        private readonly IRacesService racesService;

        public RacesController(IRacesService racesService)
        {
            this.racesService = racesService;
        }
        [HttpGet]
        public async Task<ActionResult<RaceDto>> Races()
        {
            return Ok(await racesService
                .RacesListAsync()
                .ConfigureAwait(false));
        }
    }
}