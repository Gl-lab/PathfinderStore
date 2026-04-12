using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Store.Application.Services;

public interface IProductService
{
    Task<List<Product>> SearchArticles(PageSearchArgs args);
    Task<Product> GetArticleById(int articleId);

    Task<Product> CreateArticle(string name, string description, decimal? price, decimal? weight,
                                byte categoryType);

    Task UpdateArticle(Product product);
    Task DeleteArticle(Product product);
}