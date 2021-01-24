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
using Microsoft.AspNetCore.Authorization;
using Pathfinder.Core.Entities.Auth.Permissions;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IArticleService productService;

        public ProductsController(IArticleService productService)
        {
            this.productService = productService;
        }

        [Produces("application/json")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ArticleModel>>> GetProducts()
        {
            var products = await productService
                                .GetArticleList()
                                .ConfigureAwait(false);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(IPagedList<ArticleModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IPagedList<ArticleModel>>> SearchProducts(PageSearchArgs arg)
        {
            var productPagedList = await productService
                                    .SearchArticles(arg)
                                    .ConfigureAwait(false);
            return Ok(productPagedList);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(ArticleModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ArticleModel>> GetProductById(int id)
        {
            var product = await productService
                            .GetArticleById(id)
                            .ConfigureAwait(false);
            return Ok(product);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ArticleModel>>> GetProductsByName(string name)
        {
            var products = await productService
                                .GetArticlesByName(name)
                                .ConfigureAwait(false);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ArticleModel>>> GetProductsByCategoryId(int categoryId)
        {
            var products = await productService
                                .GetArticlesByCategoryId(categoryId)
                                .ConfigureAwait(false);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(ArticleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        [Authorize]
        public async Task<ActionResult<ArticleModel>> CreateProduct(ArticleModel product)
        {
            var Result = await productService
                            .CreateArticle(product)
                            .ConfigureAwait(false);
            return Ok(Result);
        }

        [Route("[action]")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> UpdateProduct(ArticleModel product)
        {
            await productService
                .UpdateArticle(product)
                .ConfigureAwait(false);
            return Ok();
        }

        [Route("[action]")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> DeleteProductById(ArticleModel product)
        {
            await productService
                .DeleteArticleById(product.Id)
                .ConfigureAwait(false);
            return Ok();
        }
    }
}