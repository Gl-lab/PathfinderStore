using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Paging;
using Pathfinder.Core.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IEnumerable<Article>> GetListAsync();
        Task<IPagedList<Article>> SearchAsync(PageSearchArgs args);
        Task<IEnumerable<Article>> GetListByNameAsync(string productName);
        Task<Article> GetByIdWithCategoryAsync(int productId);
        Task<IEnumerable<Article>> GetListByCategoryAsync(int categoryId);
    }
}
