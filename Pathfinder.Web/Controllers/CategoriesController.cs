using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Core.Paging;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Models;
using System.Net;

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
        [ProducesResponseType(typeof(IEnumerable<CategoryModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CategoryModel>>> Categories()
        {
            var categories = await CategoryService.GetCategoryList ().ConfigureAwait(false);
            return Ok(categories);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(IPagedList<CategoryModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IPagedList<CategoryModel>>> SearchCategories(PageSearchArgs arg)
        {
            var categoryPagedList = await CategoryService.SearchCategories(arg).ConfigureAwait(false);
            return Ok(categoryPagedList);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(CategoryModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CategoryModel>> GetCategoryById(int id)
        {
            var product = await CategoryService.GetById(id).ConfigureAwait(false);
            return Ok(product);
        }
    }
}