using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Services;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.UseCases.Products;

public class SearchArticlesHandler : IRequestHandler<SearchArticlesCommand, List<ProductDto>>
{
    private readonly IProductService _productService;

    public SearchArticlesHandler( IProductService productService )
    {
        _productService = productService;
    }

    public async Task<List<ProductDto>> Handle( SearchArticlesCommand request, CancellationToken cancellationToken )
    {
        List<Product> products = await _productService
                                      .SearchArticles( request.SearchParams )
                                      .ConfigureAwait( false );

        //List<ProductDto> productModels = _mapper.Map<List<ProductDto>>(products.Items);
        throw new NotImplementedException();
        //return new List<ProductDto>(
        // products.PageIndex,
        // products.PageSize,
        // products.TotalCount,
        // products.TotalPages,
        //productModels);
    }
}