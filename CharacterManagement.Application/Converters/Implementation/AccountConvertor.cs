using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters.Implementation;

public class AccountConvertor : IAccountConvertor
{
    private readonly ICharacterConvertor _characterConvertor;

    public AccountConvertor( ICharacterConvertor characterConvertor )
    {
        _characterConvertor = characterConvertor;
    }

    public AccountDto Convert( Account account )
    {
        return new AccountDto()
        {
            Name = account.Name,
            Surname = account.Surname,
            UserId = account.UserId,

            //Characters = account.Characters.ConvertAll( x => _characterConvertor.Convert( x ) )
        };
    }

    public Account Convert( AccountDto account )
    {
        throw new NotImplementedException();
    }
}