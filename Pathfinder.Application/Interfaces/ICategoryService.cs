using Pathfinder.Application.Models;
using Pathfinder.Utils.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryModel>> GetCategoryList();
        Task<CategoryModel> GetById(int id);
        Task<IPagedList<CategoryModel>> SearchCategories(PageSearchArgs args);
    }
}
