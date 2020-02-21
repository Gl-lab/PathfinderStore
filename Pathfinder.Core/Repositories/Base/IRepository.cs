using Pathfinder.Core.Entities.Base;

namespace Pathfinder.Core.Repositories.Base
{
    public interface IRepository<T> : IRepositoryBase<T, int> where T : IEntityBase<int>
    {
    }
}
