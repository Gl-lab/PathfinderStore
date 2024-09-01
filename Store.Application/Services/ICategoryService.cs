using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Store.Application.Services;

public interface ICategoryService
{
    Task<ICollection<Category>> GetCategoryList();
    Task<Category> Get(CategoryType categoryType);
}