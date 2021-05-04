using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using System.Net;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [Produces("application/json")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Categories()
        {
            var categories = await categoryService.GetCategoryList().ConfigureAwait(false);
            return Ok(categories);
        }

        [Route("{categoryType}")]
        [HttpGet]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(byte categoryType)
        {
            var product = await categoryService.Get((CategoryType)categoryType).ConfigureAwait(false);
            return Ok(product);
        }
    }
}