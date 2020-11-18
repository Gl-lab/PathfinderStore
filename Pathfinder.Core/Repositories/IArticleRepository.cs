using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IEnumerable<Article>> GetArticleListAsync();
        Task<IPagedList<Article>> SearchArticlesAsync(PageSearchArgs args);
        Task<IEnumerable<Article>> GetArticleByNameAsync(string productName);
        Task<Article> GetArticleByIdWithCategoryAsync(int productId);
        Task<IEnumerable<Article>> GetArticleByCategoryAsync(int categoryId);
    }
}
