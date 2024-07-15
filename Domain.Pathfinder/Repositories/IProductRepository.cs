using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Paging;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetListAsync();
        Task<List<Product>> SearchAsync(PageSearchArgs args);
        Task<List<Product>> SearchByNameAsync(string productName);
        Task<Product> GetByIdWithCategoryAsync(int productId);
        Task<List<Product>> GetListByCategoryAsync(CategoryType categoryType);

        Task<Product> GetByName(string name);
    }
}