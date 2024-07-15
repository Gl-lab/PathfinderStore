using CharacterManagement.Application.DTO;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Converters;

public interface IAccountConvertor
{
    public AccountDto Convert( Account account );
    public Account Convert( AccountDto account );
}