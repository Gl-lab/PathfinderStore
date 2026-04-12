using System.Threading.Tasks;

namespace Pathfinder.Utils.UnitOfWork;

public interface IUnitOfWork
{
    Task Commit();
    Task Rollback();
    Task BeginTransaction();
}