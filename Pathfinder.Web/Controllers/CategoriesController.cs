using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using System.Net;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService CategoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.CategoryService = categoryService;
        }

        [Produces("application/json")]
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Categories()
        {
            var categories = await CategoryService.GetCategoryList ().ConfigureAwait(false);
            return Ok(categories);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(IPagedList<CategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IPagedList<CategoryDto>>> SearchCategories(PageSearchArgs arg)
        {
            var categoryPagedList = await CategoryService.SearchCategories(arg).ConfigureAwait(false);
            return Ok(categoryPagedList);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var product = await CategoryService.GetById(id).ConfigureAwait(false);
            return Ok(product);
        }
    }
}