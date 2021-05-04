using System.Collections;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Paging;
using Pathfinder.Core.Repositories.Base;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface ICategoryRepository 
    {
        Task<ICollection<Category>> ListAsync();
        Task<Category> GetAsync(CategoryType categoryType);
    }
}
