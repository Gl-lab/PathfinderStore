using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.UseCases.Articles;

public class SearchArticlesCommand: IRequest<IPagedList<ArticleDto>>
{
    public SearchArticlesCommand(PageSearchArgs searchParams)
    {
        SearchParams = searchParams;
    }

    public PageSearchArgs SearchParams { get; }
}