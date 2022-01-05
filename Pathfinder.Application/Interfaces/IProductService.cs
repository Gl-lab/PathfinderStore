using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.Interfaces
{
    public interface IProductService
    {
        Task<IPagedList<Product>> SearchArticles(PageSearchArgs args);
        Task<Product> GetArticleById(int articleId);

        Task<Product> CreateArticle(string name, string description, decimal? price, decimal? weight,
            byte categoryType);

        Task UpdateArticle(Product product);
        Task DeleteArticle(Product product);
    }
}