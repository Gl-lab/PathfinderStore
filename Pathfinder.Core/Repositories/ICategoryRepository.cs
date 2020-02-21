using Pathfinder.Core.Entities;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories.Base;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IPagedList<Category>> SearchCategoriesAsync(PageSearchArgs args);
    }
}
