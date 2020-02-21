using Pathfinder.Core.Entities;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductListAsync();
        Task<IPagedList<Product>> SearchProductsAsync(PageSearchArgs args);
        Task<IEnumerable<Product>> GetProductByNameAsync(string productName);
        Task<Product> GetProductByIdWithCategoryAsync(int productId);
        Task<IEnumerable<Product>> GetProductByCategoryAsync(int categoryId);
    }
}
