using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Interfaces
{
    public interface ICharacterService
    {
        Task<Character> GetCharacterAsync(int userId);
        Task<int> IncreaseBalance(int userId, int balance);
        Task<int> DecreaseBalance(int userId, int balance);
        Task<ICollection<WeaponItemProperty>> WeaponItemProperty(int userId);
    }
}