using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class TeamsController: AuthorizedController
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok();
        }
        
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> Teammates()
        {
            return Ok();
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateTeam()
        {
            return Ok();
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> LeaveTeam()
        {
            return Ok();
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteTeam()
        {
            return Ok();
        }
        
        [HttpPut]
        public async Task<ActionResult> EditTeam()
        {
            return Ok();
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> JoinToTeam()
        {
            return Ok();
        }
    }
}