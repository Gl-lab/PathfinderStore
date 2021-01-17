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
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Services
{
    public class CharacterService: ICharacterService
    {
        private readonly IMapper mapper;
        private readonly IAccountRepository accountRepository;
        private readonly ICharacterRepository characterRepository;
        public CharacterService(IAccountRepository accountRepository, IMapper mapper, ICharacterRepository characterRepository)
        {
            this.accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
        }

        public async Task<CharacterDto> GetCurrentCharacter(User user)
        {
            var character = await characterRepository.GetCurrentAsync(user.Id).ConfigureAwait(false);
            return mapper.Map<CharacterDto>(character);
        }

        public async Task<ICollection<CharacterDto>> GetCharactersByUserId(User user)
        {
            var characters = await characterRepository.GetListAsync(user.Id).ConfigureAwait(false);
            return mapper.Map<ICollection<CharacterDto>>(characters);
        }

        public async Task SetCurrentCharacter(User user, int characterId)
        {
            var account = await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);
            if (account == null) return;
            var character = await characterRepository.GetByIdAsync(characterId).ConfigureAwait(false);
            if (character == null) return;
            if (!account.Characters.Contains(character)) return;
            account.CurrentCharacter = character;
            await accountRepository.SaveAsync(account).ConfigureAwait(false);
            return;
        }

        public async Task<CharacterDto> CreateCharacter(User user, CharacterDto newCharacter)
        {
            var account = await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);

            if (account.Characters.Any(e => e.Name == newCharacter.Name))
            {
                throw new ApplicationException("Герой с таким именем уже существует");
            }
            account.Characters.Add(mapper.Map<Character>(newCharacter));
            var result = await accountRepository.SaveAsync(account).ConfigureAwait(false);

            return mapper.Map<CharacterDto>(result.Characters.First(e => e.Name == newCharacter.Name));
        }

        public async Task DeleteCharacter(User user, CharacterDto character)
        {
            var account = await accountRepository.GetByUserIdAsync(user.Id).ConfigureAwait(false);

            if (account.Characters.All(e => e.Id != character.Id))
            {
                throw new ApplicationException("Герой отсутствует");
            }
            account.Characters.Remove(mapper.Map<Character>(character));
            var result = await accountRepository.SaveAsync(account).ConfigureAwait(false);

            return;
        }
    }
}
