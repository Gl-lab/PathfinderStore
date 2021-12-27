using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO;
using Pathfinder.Application.UseCases.Products;
using Pathfinder.Core.Entities.Authentication.Permissions;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(IPagedList<ProductDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IPagedList<ProductDto>>> SearchProducts(PageSearchArgs arg)
        {
            var result = await _mediator.Send(new SearchArticlesCommand(arg));
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var result = await _mediator.Send(new ArticleByIdCommand(id));
            return Ok(result);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateArticleCommand product)
        {
            var result = await _mediator.Send(product);
            return Ok(result);
        }

        [Route("[action]")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> UpdateProduct(ProductDto product)
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
        public async Task<ActionResult> DeleteProductById(ProductDto product)
        {
            await _mediator.Send(new DeleteArticleCommand(product.Id));
            return Ok();
        }
    }
}