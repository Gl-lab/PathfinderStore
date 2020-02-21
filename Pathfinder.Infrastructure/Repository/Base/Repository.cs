using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Repositories.Base;
using Pathfinder.Infrastructure.Data;

namespace Pathfinder.Infrastructure.Repository.Base
{
    public class Repository<T> : RepositoryBase<T, int>, IRepository<T>
        where T : class, IEntityBase<int>
    {
        public Repository(PgDbContext context)
            : base(context)
        {
        }
    }
}
