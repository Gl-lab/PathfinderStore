using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Paging;
using Pathfinder.Core.Repositories.Base;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IPagedList<Category>> SearchAsync(PageSearchArgs args);
    }
}
