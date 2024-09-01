using Microsoft.EntityFrameworkCore;
using Pathfinder.Utils.Entities.Base;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.Shared.Infrasturture.Repositories;

public class Repository<T> : RepositoryBase<T, int>, IRepository<T>
    where T : class, IEntityBase<int>
{
    public Repository( DbContext context )
        : base( context )
    {
    }
}