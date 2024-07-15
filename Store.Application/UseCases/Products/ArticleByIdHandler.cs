using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Services;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.UseCases.Products;

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