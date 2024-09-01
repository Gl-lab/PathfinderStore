using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Utils.Repositories.Base;

public interface IEnumRepository<T> : IRepositoryBase<T, int> where T : IEntityBase<int>
{
}