using Pathfinder.Application.Models;
using Pathfinder.Core.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryModel>> GetCategoryList();

        Task<IPagedList<CategoryModel>> SearchCategories(PageSearchArgs args);
    }
}
