using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Repositories;
using AutoMapper;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Auth.Users;
using System.Linq;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Services
{
    public sealed class AccountService: IAccountService
    {
        private readonly IMapper mapper;
        private readonly IAccountRepository accountRepository;
        private readonly IUserService userService;
        public AccountService(IAccountRepository accountRepository, 
            IMapper mapper,
            IUserService userService)
        {
            this.accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<AccountDto> GetCurrentAccountAsync()
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            return mapper.Map<AccountDto>(account);
        }

        public async Task CreateCharacterAsync(CharacterDto newCharacter)
        {
            if (newCharacter == null) throw new ArgumentNullException(nameof(newCharacter));
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            if (account.Characters.Any(e => e.Name == newCharacter.Name))
            {
                throw new ApplicationException("Герой с таким именем уже существует");
            }
            account.Characters.Add(mapper.Map<Character>(newCharacter));
            await accountRepository.SaveAsync(account).ConfigureAwait(false);
        }

        public async Task DeleteCharacterAsync(int deletedCharacterId)
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            var character = account.Characters.FirstOrDefault(e => e.Id == deletedCharacterId);
            if (character == null)
            {
                throw new ApplicationException("Герой отсутствует");
            }
            account.Characters.Remove(character);
            await accountRepository.SaveAsync(account).ConfigureAwait(false);
        }

        public async Task<ICollection<CharacterDto>> GetCharactersByCurrentUserAsync()
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            return mapper.Map<ICollection<CharacterDto>>(account.Characters);
        }

        public async Task<ICollection<CharacterDto>> GetCharactersByUserAsync(User user)
        {
            var account = await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);
            if (account == null) throw new Exception("Account by user not exists");
            return mapper.Map<ICollection<CharacterDto>>(account.Characters);
        }

        public async Task SetCurrentCharacterAsync(int characterId)
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            var character = account.Characters.FirstOrDefault(e => e.Id == characterId);
            account.CurrentCharacter = character ?? throw new Exception("The user does not have a character with the specified ID");
            await accountRepository.SaveAsync(account).ConfigureAwait(false);
        }

        public async Task SetCurrentCharacterAsync(CharacterDto character)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            var accountCharacter = account.Characters.FirstOrDefault(e => e.Id == character.Id);
            account.CurrentCharacter = accountCharacter ?? throw new Exception("The user does not have a character with the specified ID");
            await accountRepository.SaveAsync(account).ConfigureAwait(false);
        }
       
        public async Task UpdateAsync(AccountDto newAccount)
        {
            var user = userService.GetCurrentUser()
                ?? throw new Exception("userService.GetCurrentUser()");
            var account = await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);
            if (account == null) throw new Exception("Account for user not exists");
            account.Name = newAccount.Name;
            account.Surname = newAccount.Surname;
            await accountRepository.SaveAsync(account).ConfigureAwait(false);
        }

        private async Task<Account> CurrentAccountAsync()
        {
            var user = userService.GetCurrentUser();
            return await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);
        }
    }
}