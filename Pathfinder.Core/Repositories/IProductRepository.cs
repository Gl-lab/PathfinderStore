using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories.Base;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetListAsync();
        Task<IPagedList<Product>> SearchAsync(PageSearchArgs args);
        Task<IEnumerable<Product>> SearchByNameAsync(string productName);
        Task<Product> GetByIdWithCategoryAsync(int productId);
        Task<IEnumerable<Product>> GetListByCategoryAsync(CategoryType categoryType);

        Task<Product> GetByName(string name);
    }
}