using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Store.Application.DTO;
using Pathfinder.Store.Application.Services;

namespace Pathfinder.Store.Application.UseCases.Products;

public class ArticleByIdHandler : IRequestHandler<ArticleByIdCommand, ProductDto>
{
    private readonly IProductService _productService;

    public ArticleByIdHandler( IProductService productService )
    {
        _productService = productService;
    }

    public async Task<ProductDto> Handle( ArticleByIdCommand request, CancellationToken cancellationToken )
    {
        Product article = await _productService.GetArticleById( request.Id );
        throw new NotImplementedException();
    }
}