using System.Threading.Tasks;

namespace Pathfinder.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task RollbackAsync();
        Task BeginTransaction();
    }
}