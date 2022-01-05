using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Services
{
    public sealed class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IWeaponItemPropertyRepository _weaponItemPropertyRepository;

        public CharacterService(ICharacterRepository characterRepository,
            IWeaponItemPropertyRepository weaponItemPropertyRepository)
        {
            _characterRepository = characterRepository;
            _weaponItemPropertyRepository = weaponItemPropertyRepository;
        }

        public async Task<Character> GetCharacterAsync(int userId)
        {
            return await _characterRepository.GetCurrentAsync(userId);
        }

        public async Task<int> IncreaseBalance(int userId, int balance)
        {
            var character = await _characterRepository.GetCurrentAsync(userId);
            character.Backpack.Wallet.IncreaseBalance(balance);
            return character.Backpack.Wallet.Balance;
        }

        public async Task<int> DecreaseBalance(int userId, int balance)
        {
            var character = await _characterRepository.GetCurrentAsync(userId);
            character.Backpack.Wallet.DecreaseBalance(balance);
            return character.Backpack.Wallet.Balance;
        }

        public async Task EditCharacter(CharacterDto newCharacter)
        {
            // var character = await _characterRepository.GetCurrentAsync(_userService.GetCurrentUser().Id);
            // if (newCharacter.Name != character.Name ) character.Rename(newCharacter.Name);
            // if (newCharacter.RaceId != character.RaceId) character.ChangeRace(newCharacter.RaceId);
        }

        public async Task<ICollection<WeaponItemProperty>> WeaponItemProperty(int userId)
        {
            var character = await _characterRepository.GetCurrentAsync(userId);
            if (character == null) throw new ApplicationException("User dont have current character");
            return await _weaponItemPropertyRepository.GetWeaponItemByItemIdCollection(
                character
                    .Backpack
                    .Items
                    .Where(e => e.Item.Product.CategoryType == CategoryType.Weapon)
                    .Select(e => e.Item.Id)
                    .ToList());
        }
    }
}