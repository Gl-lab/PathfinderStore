using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers.Shop;

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
        // IReadOnlyList<ShopDto> result = await _mediator.Send(new GetAllShopsCommand());
        return Ok();
    }
        
    [HttpPost]
    [Route("{shopId}/[action]")]
    public async Task<ActionResult> Buy() => Ok();

    [HttpPost]
    [Route("{shopId}/[action]")]
    public async Task<ActionResult> Sell() => Ok();
}