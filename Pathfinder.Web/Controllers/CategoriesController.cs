using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.UseCases.Category;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Produces("application/json")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Categories()
        {
            var categories = await _mediator.Send(new GetCategoriesCommand());
            return Ok(categories);
        }

        [Route("{categoryType}")]
        [HttpGet]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(byte categoryType)
        {
            var category = await _mediator.Send(new CategoryByCategoryTypeCommand(categoryType));
            return Ok(category);
        }
    }
}