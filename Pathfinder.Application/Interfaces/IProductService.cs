using Pathfinder.Application.DTO;
using Pathfinder.Utils.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDto>> GetArticleList();
        Task<IPagedList<ArticleDto>> SearchArticles(PageSearchArgs args);
        Task<ArticleDto> GetArticleById(int ArticleId);
        Task<IEnumerable<ArticleDto>> GetArticlesByName(string name);
        Task<IEnumerable<ArticleDto>> GetArticlesByCategoryId(int categoryId);
        Task<ArticleDto> CreateArticle(ArticleDto Article);
        Task UpdateArticle(ArticleDto Article);
        Task DeleteArticleById(int ArticleId);
    }
}
