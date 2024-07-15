using System;
using System.Linq;
using System.Threading.Tasks;
using Pathfinder.Application.Exceptions;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Services.Implementation
{
    public sealed class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        // private readonly ICharacterConvertor _characterConvertor;
        // private readonly IAccountConvertor _accountConvertor;

        public AccountService( IAccountRepository accountRepository )

        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException( nameof( accountRepository ) );
        }

        public async Task CreateAsync( int userId )
        {
            Account account = await _accountRepository.GetByUserIdAsync( userId );
            if ( account is null )
            {
                throw new PathfiderApplicationException( $"Account already exists for user {userId}" );
            }

            _accountRepository.Add( new Account() );
        }

        // public async Task CreateCharacterAsync( CharacterDto newCharacter )
        // {
        //     
        //     if ( newCharacter == null )
        //     {
        //         throw new ArgumentNullException( nameof( newCharacter ) );
        //     }
        //     
        //     Account account = await CurrentAccountAsync().ConfigureAwait( false );
        //     if ( account.Characters.Any( e => e.Name == newCharacter.Name ) )
        //     {
        //         throw new PathfiderApplicationException( "Герой с таким именем уже существует" );
        //     }
        //     
        //     account.Characters.Add( _characterConvertor.Convert( newCharacter ) );
        //     _accountRepository.Save( account );
        // }

        public async Task DeleteCharacterAsync( int deletedCharacterId )
        {
            Account? account = await _accountRepository.GetByCharacterIdAsync( deletedCharacterId );
            if ( account is null )
            {
                throw new PathfiderApplicationException( $"Account with character {deletedCharacterId} not found" );
            }

            Character? character = account.Characters.FirstOrDefault( e => e.Id == deletedCharacterId );
            if ( character == null )
            {
                throw new PathfiderApplicationException( $"Character with id {deletedCharacterId} not found" );
            }

            account.Characters.Remove( character );
        }

        // public async Task UpdateAsync( AccountDto newAccount )
        // {
        //     Account account = await _accountRepository.GetByUserIdAsync( newAccount.UserId );
        //     if ( account == null )
        //     {
        //         throw new PathfiderApplicationException( "Account for user not exists" );
        //     }
        //
        //     account.Name = newAccount.Name;
        //     account.Surname = newAccount.Surname;
        // }
    }
}