using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Utils.Paging;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Pathfinder.Application.UseCases.Articles;
using Pathfinder.Core.Entities.Authentication.Permissions;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IProductService productService, IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(IPagedList<ArticleDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IPagedList<ArticleDto>>> SearchProducts(PageSearchArgs arg)
        {
            var result = await _mediator.Send(new SearchArticlesCommand(arg));
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ArticleDto>> GetProductById(int id)
        {
            var result = await _mediator.Send(new ArticleByIdCommand(id));
            return Ok(result);
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
            await _mediator.Send(new UpdateArticleCommand(product.Id, product.Name, product.Description,
                product.Price, product.Weight, product.CategoryType));
            return Ok();
        }

        [Route("[action]")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> DeleteProductById(ArticleDto product)
        {
            await _mediator.Send(new DeleteArticleCommand(product.Id));
            return Ok();
        }
    }
}