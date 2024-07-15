using System.Threading.Tasks;

namespace Pathfinder.Application.Services
{
    public interface IAccountService
    {
        Task CreateAsync( int userId );
        //Task CreateCharacterAsync( CharacterDto newCharacter );
        Task DeleteCharacterAsync( int deletedCharacterId );
    }
}