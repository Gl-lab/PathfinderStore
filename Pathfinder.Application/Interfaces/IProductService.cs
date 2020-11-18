using Pathfinder.Application.Models;
using Pathfinder.Core.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleModel>> GetArticleList();
        Task<IPagedList<ArticleModel>> SearchArticles(PageSearchArgs args);
        Task<ArticleModel> GetArticleById(int ArticleId);
        Task<IEnumerable<ArticleModel>> GetArticlesByName(string name);
        Task<IEnumerable<ArticleModel>> GetArticlesByCategoryId(int categoryId);
        Task<ArticleModel> CreateArticle(ArticleModel Article);
        Task UpdateArticle(ArticleModel Article);
        Task DeleteArticleById(int ArticleId);
    }
}
