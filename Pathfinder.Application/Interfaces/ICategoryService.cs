using Pathfinder.Application.DTO;
using Pathfinder.Utils.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoryList();
        Task<CategoryDto> GetById(int id);
        Task<IPagedList<CategoryDto>> SearchCategories(PageSearchArgs args);
    }
}
