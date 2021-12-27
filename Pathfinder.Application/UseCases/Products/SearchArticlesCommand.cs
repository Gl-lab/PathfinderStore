using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.UseCases.Products;

public class SearchArticlesCommand : IRequest<IPagedList<ProductDto>>
{
    public SearchArticlesCommand(PageSearchArgs searchParams)
    {
        SearchParams = searchParams;
    }

    public PageSearchArgs SearchParams { get; }
}