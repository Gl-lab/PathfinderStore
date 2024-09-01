using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public interface IAccountConvertor
{
    public AccountDto Convert( Account account );
    public Account Convert( AccountDto account );
}