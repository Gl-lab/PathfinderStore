using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.UseCases.Products;

public class SearchArticlesHandler : IRequestHandler<SearchArticlesCommand, IPagedList<ProductDto>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public SearchArticlesHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<IPagedList<ProductDto>> Handle(SearchArticlesCommand request, CancellationToken cancellationToken)
    {
        var products = await _productService
            .SearchArticles(request.SearchParams)
            .ConfigureAwait(false);

        var productModels = _mapper.Map<List<ProductDto>>(products.Items);

        return new PagedList<ProductDto>(
            products.PageIndex,
            products.PageSize,
            products.TotalCount,
            products.TotalPages,
            productModels);
    }
}