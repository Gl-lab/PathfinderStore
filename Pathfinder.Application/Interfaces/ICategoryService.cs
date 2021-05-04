using Pathfinder.Application.DTO;
using Pathfinder.Utils.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoryList();
        Task<CategoryDto> Get(CategoryType categoryType);
    }
}
