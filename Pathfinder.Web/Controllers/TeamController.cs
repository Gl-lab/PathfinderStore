using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

public class TeamsController: AuthorizedController
{
    [HttpGet]
    public async Task<ActionResult> Get() => Ok();

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult> Teammates() => Ok();

    [HttpPost]
    public async Task<ActionResult> CreateTeam() => Ok();

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> LeaveTeam() => Ok();

    [HttpDelete]
    public async Task<ActionResult> DeleteTeam() => Ok();

    [HttpPut]
    public async Task<ActionResult> EditTeam() => Ok();

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> JoinToTeam() => Ok();
}