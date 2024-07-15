using Pathfinder.Core.Entities.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.Core.Repositories
{
    public interface ICharacterRepository : IRepository<Character>
    {
        Task<List<Character>> GetListAsync(int userId);
       // Task<Character> GetCurrentAsync(int UserId);
    }
}
