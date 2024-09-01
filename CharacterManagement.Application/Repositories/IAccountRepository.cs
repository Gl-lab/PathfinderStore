using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByUserIdAsync(int userId);
    Task<Account?> GetByCharacterIdAsync(int userId);
}