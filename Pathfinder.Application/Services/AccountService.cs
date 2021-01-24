using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Mapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Interfaces;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Paging;
using AutoMapper;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Auth.Users;
using System.Linq;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Services
{
    public class AccountService: IAccountService, ICharacterService
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

        public async Task CreateCharacterAsync(CharacterDto newCharacter)
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            if (account.Characters.Any(e => e.Name == newCharacter.Name))
            {
                throw new ApplicationException("Герой с таким именем уже существует");
            }
            account.Characters.Add(mapper.Map<Character>(newCharacter));
            var result = await accountRepository.SaveAsync(account).ConfigureAwait(false);
        }

        public async Task DeleteCharacterAsync(CharacterDto character)
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            if (account.Characters.All(e => e.Id != character.Id))
            {
                throw new ApplicationException("Герой отсутствует");
            }
            account.Characters.Remove(mapper.Map<Character>(character));
            var result = await accountRepository.SaveAsync(account).ConfigureAwait(false);
            return;
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

        public async Task<AccountDto> GetCurrentAccountAsync()
        {
           var account = await CurrentAccountAsync().ConfigureAwait(false);
           return mapper.Map<AccountDto>(account);
        }

        public async Task<CharacterDto> GetCurrentCharacterAsync()
        {
           var account = await CurrentAccountAsync().ConfigureAwait(false);
           return mapper.Map<CharacterDto>(account.CurrentCharacter);
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