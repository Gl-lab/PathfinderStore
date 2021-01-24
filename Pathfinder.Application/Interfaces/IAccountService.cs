using Pathfinder.Application.DTO;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Core.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface IAccountService
    {
        Task UpdateAsync(AccountDto newAccount);
        Task<AccountDto> GetCurrentAccountAsync();
    }
}