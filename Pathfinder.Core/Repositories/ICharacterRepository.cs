using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface ICharacterRepository : IRepository<Character>
    {
        Task<ICollection<Character>> GetListAsync(int UserId);
        Task<Character> GetCurrentAsync(int UserId);
    }
}
