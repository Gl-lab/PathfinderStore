using Pathfinder.Infrastructure.Data;
using Pathfinder.Utils.Entities.Base;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.Infrastructure.Repository.Base
{
    public class Repository<T> : RepositoryBase<T, int>, IRepository<T>
        where T : class, IEntityBase<int>
    {
        public Repository(PathfinderDbContext context)
            : base(context)
        {
        }
    }
}
