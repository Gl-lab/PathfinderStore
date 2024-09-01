using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Utils.Repositories.Base;

public interface IRepository<T> : IRepositoryBase<T, int> where T : IEntityBase<int>
{
}