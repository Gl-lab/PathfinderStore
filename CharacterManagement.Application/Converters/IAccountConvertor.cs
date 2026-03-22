using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public interface IAccountConvertor
{
    AccountDto Convert( Account account );
    Account Convert( AccountDto account );
}