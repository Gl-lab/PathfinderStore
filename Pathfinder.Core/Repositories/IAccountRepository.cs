using System.Threading.Tasks;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories.Base;

namespace Pathfinder.Core.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByUserIdAsync(int userId);
    }
}