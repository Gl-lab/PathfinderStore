using CharacterManagement.Application.DTO;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Converters;

public interface ICharacterConvertor
{
    public Character Convert( CharacterDto character );
    public CharacterDto Convert( Character character );
}