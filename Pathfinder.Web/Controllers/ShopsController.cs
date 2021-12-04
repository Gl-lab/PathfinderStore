using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.UseCases.Shop;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class ShopsController: AuthorizedController
    {
        private readonly IMediator _mediator;

        public ShopsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _mediator.Send(new GetAllShopsCommand());
            return Ok(result);
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