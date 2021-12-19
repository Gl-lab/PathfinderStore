using Pathfinder.Application.DTO;
using Pathfinder.Utils.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Interfaces
{
    public interface IProductService
    {
        Task<IPagedList<Article>> SearchArticles(PageSearchArgs args);
        Task<Article> GetArticleById(int articleId);
        Task<Article> CreateArticle(string name, string description, decimal? price, decimal? weight,
            byte categoryType);
        Task UpdateArticle(Article article);
        Task DeleteArticle(Article article);
        
    }
}
