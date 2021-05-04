using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class ShopsController: AuthorizedController
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok();
        }
        
        [HttpPost]
        [Route("{shopId}/[action]")]
        public async Task<ActionResult> Buy()
        {
            return Ok();
        }
        
        [HttpPost]
        [Route("{shopId}/[action]")]
        public async Task<ActionResult> Sell()
        {
            return Ok();
        }
    }
}