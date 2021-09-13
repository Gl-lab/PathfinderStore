using Pathfinder.Application.DTO;
using Pathfinder.Utils.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDto>> GetArticleList();
        Task<IPagedList<ArticleDto>> SearchArticles(PageSearchArgs args);
        Task<ArticleDto> GetArticleById(int articleId);
        Task<IEnumerable<ArticleDto>> GetArticlesByName(string name);
        Task<IEnumerable<ArticleDto>> GetArticlesByCategoryId(CategoryType categoryType);
        Task<ArticleDto> CreateArticle(ArticleDto article);
        Task<ArticleDto> CreateArticle(string name, string description, decimal? price, decimal? weight,
            byte categoryType);
        Task UpdateArticle(ArticleDto article);
        Task DeleteArticleById(int articleId);
        
    }
}
