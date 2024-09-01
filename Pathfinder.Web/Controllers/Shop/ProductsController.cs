using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Pathfinder.Web.Controllers.Shop;

[Route( "api/[controller]" )]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController( IMediator mediator )
    {
        _mediator = mediator;
    }

    // [Route( "[action]" )]
    // [HttpPost]
    // [ProducesResponseType( typeof( List<ProductDto> ), ( int )HttpStatusCode.OK )]
    // public async Task<ActionResult<List<ProductDto>>> SearchProducts( PageSearchArgs arg )
    // {
    //     List<ProductDto> result = await _mediator.Send( new SearchArticlesCommand( arg ) );
    //     return Ok( result );
    // }
    //
    // [Route( "{id:int}" )]
    // [HttpGet]
    // [ProducesResponseType( typeof( ProductDto ), ( int )HttpStatusCode.OK )]
    // public async Task<ActionResult<ProductDto>> GetProductById( int id )
    // {
    //     ProductDto result = await _mediator.Send( new ArticleByIdCommand( id ) );
    //     return Ok( result );
    // }
    //
    // [Route( "[action]" )]
    // [HttpPost]
    // [ProducesResponseType( typeof( ProductDto ), ( int )HttpStatusCode.OK )]
    // [ProducesResponseType( ( int )HttpStatusCode.BadRequest )]
    // [Authorize( Policy = DefaultPermissions.PermissionNameForAdministration )]
    // public async Task<ActionResult<ProductDto>> CreateProduct( CreateArticleCommand product )
    // {
    //     ProductDto result = await _mediator.Send( product );
    //     return Ok( result );
    // }
    //
    // [Route( "[action]" )]
    // [HttpPut]
    // [ProducesResponseType( ( int )HttpStatusCode.OK )]
    // [ProducesResponseType( ( int )HttpStatusCode.BadRequest )]
    // [Authorize( Policy = DefaultPermissions.PermissionNameForAdministration )]
    // public async Task<ActionResult> UpdateProduct( ProductDto product )
    // {
    //     await _mediator.Send( new UpdateArticleCommand( product.Id, product.Name, product.Description,
    //         product.Price, product.Weight, product.CategoryType ) );
    //     return Ok();
    // }
    //
    // [Route( "[action]" )]
    // [HttpDelete]
    // [ProducesResponseType( ( int )HttpStatusCode.OK )]
    // [ProducesResponseType( ( int )HttpStatusCode.BadRequest )]
    // [Authorize( Policy = DefaultPermissions.PermissionNameForAdministration )]
    // public async Task<ActionResult> DeleteProductById( ProductDto product )
    // {
    //     await _mediator.Send( new DeleteArticleCommand( product.Id ) );
    //     return Ok();
    // }
}