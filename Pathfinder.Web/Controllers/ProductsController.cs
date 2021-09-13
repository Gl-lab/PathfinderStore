using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Utils.Paging;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Pathfinder.Application.UseCases.Articles;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IArticleService _productService;
        private readonly IMediator _mediator;

        public ProductsController(IArticleService productService, IMediator mediator)
        {
            _productService = productService;
            _mediator = mediator;
        }

        [Produces("application/json")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetProducts()
        {
            var products = await _productService
                                .GetArticleList()
                                .ConfigureAwait(false);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(IPagedList<ArticleDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IPagedList<ArticleDto>>> SearchProducts(PageSearchArgs arg)
        {
            var productPagedList = await _productService
                                    .SearchArticles(arg)
                                    .ConfigureAwait(false);
            return Ok(productPagedList);
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ArticleDto>> GetProductById(int id)
        {
            var product = await _productService
                            .GetArticleById(id)
                            .ConfigureAwait(false);
            return Ok(product);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetProductsByName(string name)
        {
            var products = await _productService
                                .GetArticlesByName(name)
                                .ConfigureAwait(false);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetProductsByCategoryId(byte categoryType)
        {
            var products = await _productService
                                .GetArticlesByCategoryId((CategoryType)categoryType)
                                .ConfigureAwait(false);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult<ArticleDto>> CreateProduct(CreateArticleCommand product)
        {
            var result = await _mediator.Send(product);
            return Ok(result);
        }

        [Route("[action]")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> UpdateProduct(ArticleDto product)
        {
            await _productService
                .UpdateArticle(product)
                .ConfigureAwait(false);
            return Ok();
        }

        [Route("[action]")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> DeleteProductById(ArticleDto product)
        {
            await _productService
                .DeleteArticleById(product.Id)
                .ConfigureAwait(false);
            return Ok();
        }
    }
}