using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Pathfinder.Web.Controllers.Shop;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    //
    // [Produces("application/json")]
    // [HttpGet]
    // [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
    // public async Task<ActionResult<IEnumerable<CategoryDto>>> Categories()
    // {
    //     ICollection<CategoryDto> categories = await _mediator.Send(new GetCategoriesCommand());
    //     return Ok(categories);
    // }
    //
    // [Route("{categoryType}")]
    // [HttpGet]
    // [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
    // public async Task<ActionResult<CategoryDto>> GetCategoryById(byte categoryType)
    // {
    //     CategoryDto category = await _mediator.Send(new CategoryByCategoryTypeCommand(categoryType));
    //     return Ok(category);
    // }
}