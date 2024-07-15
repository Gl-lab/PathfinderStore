using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Services
{
    public interface ICategoryService
    {
        Task<ICollection<Category>> GetCategoryList();
        Task<Category> Get(CategoryType categoryType);
    }
}