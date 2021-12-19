using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.UnitOfWork;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.UseCases.Articles;

public class SearchArticlesHandler: IRequestHandler<SearchArticlesCommand, IPagedList<ArticleDto>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public SearchArticlesHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    
    public async Task<IPagedList<ArticleDto>> Handle(SearchArticlesCommand request, CancellationToken cancellationToken)
    {
        var products = await _productService

            .SearchArticles(request.SearchParams)
            .ConfigureAwait(false);
        
        var productModels = _mapper.Map<List<ArticleDto>>(products.Items);

        return new PagedList<ArticleDto>(
            products.PageIndex,
            products.PageSize,
            products.TotalCount,
            products.TotalPages,
            productModels);
    }
}