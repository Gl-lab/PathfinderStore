using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Exceptions;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Authentication.User;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Services
{
    public sealed class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserService _userService; // TODO Вытащить от сюда этот сервис

        public AccountService(IAccountRepository accountRepository,
            IMapper mapper,
            IUserService userService)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public Task CreateAsync(int userId)
        {
            _accountRepository.Add(new Account { UserId = userId });
            return Task.CompletedTask;
        }

        public async Task<AccountDto> GetCurrentAccountAsync()
        {
            var account = await CurrentAccountAsync();
            return _mapper.Map<AccountDto>(account);
        }

        public async Task CreateCharacterAsync(CharacterDto newCharacter)
        {
            if (newCharacter == null) throw new ArgumentNullException(nameof(newCharacter));
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            if (account.Characters.Any(e => e.Name == newCharacter.Name))
            {
                throw new PathfiderApplicationException("Герой с таким именем уже существует");
            }

            account.Characters.Add(_mapper.Map<Character>(newCharacter));
            _accountRepository.Save(account);
        }

        public async Task DeleteCharacterAsync(int deletedCharacterId)
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            var character = account.Characters.FirstOrDefault(e => e.Id == deletedCharacterId);
            if (character == null)
            {
                throw new PathfiderApplicationException("Герой отсутствует");
            }

            account.Characters.Remove(character);
            _accountRepository.Save(account);
        }

        public async Task<ICollection<CharacterDto>> GetCharactersByCurrentUserAsync()
        {
            var account = await CurrentAccountAsync().ConfigureAwait(false);
            return _mapper.Map<ICollection<CharacterDto>>(account.Characters);
        }

        public async Task<ICollection<CharacterDto>> GetCharactersByUserAsync(User user)
        {
            var account = await _accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);
            if (account == null) throw new PathfiderApplicationException("Account by user not exists");
            return _mapper.Map<ICollection<CharacterDto>>(account.Characters);
        }

        public async Task SetCurrentCharacterAsync(int characterId)
        {
            var account = await CurrentAccountAsync();
            var character = account.Characters.FirstOrDefault(e => e.Id == characterId);
            account.CurrentCharacter = character ??
                                       throw new PathfiderApplicationException(
                                           "The user does not have a character with the specified ID");
            _accountRepository.Save(account);
        }

        public async Task SetCurrentCharacterAsync(CharacterDto character)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            var account = await CurrentAccountAsync();
            var accountCharacter = account.Characters.FirstOrDefault(e => e.Id == character.Id);
            account.CurrentCharacter = accountCharacter ??
                                       throw new PathfiderApplicationException(
                                           "The user does not have a character with the specified ID");
            _accountRepository.Save(account);
        }

        public async Task UpdateAsync(AccountDto newAccount)
        {
            var user = _userService.GetCurrentUser()
                       ?? throw new PathfiderApplicationException("userService.GetCurrentUser()");
            var account = await _accountRepository.GetByUserIdAsync(user.Id);
            if (account == null) throw new PathfiderApplicationException("Account for user not exists");
            account.Name = newAccount.Name;
            account.Surname = newAccount.Surname;
            _accountRepository.Save(account);
        }

        private async Task<Account> CurrentAccountAsync()
        {
            var user = _userService.GetCurrentUser();
            return await _accountRepository.GetByUserIdAsync(user.Id);
        }
    }
}