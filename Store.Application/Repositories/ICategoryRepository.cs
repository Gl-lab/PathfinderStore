using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Store.Application.Repositories;

public interface ICategoryRepository 
{
    Task<ICollection<Category>> ListAsync();
    Task<Category> GetAsync(CategoryType categoryType);
}