using Pathfinder.Application.DTO;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface IAccountService
    {
        Task UpdateAsync(AccountDto newAccount);
        Task<AccountDto> GetCurrentAccountAsync();
    }
}
