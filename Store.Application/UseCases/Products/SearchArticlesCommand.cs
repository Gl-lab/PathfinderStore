using System.Collections.Generic;
using MediatR;
using Pathfinder.Store.Application.DTO;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Store.Application.UseCases.Products;

public class SearchArticlesCommand : IRequest<List<ProductDto>>
{
    public SearchArticlesCommand(PageSearchArgs searchParams)
    {
        SearchParams = searchParams;
    }

    public PageSearchArgs SearchParams { get; }
}