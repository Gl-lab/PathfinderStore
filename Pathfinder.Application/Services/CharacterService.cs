using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Items;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Services
{
    public sealed class CharacterService: ICharacterService
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly ICharacterRepository characterRepository;
        private readonly IWeaponItemPropertyRepository weaponItemPropertyRepository;
        private readonly IWeaponRepository weaponRepository;

        public CharacterService(IUserService userService, 
            IMapper mapper,
            ICharacterRepository characterRepository,
            IWeaponItemPropertyRepository weaponItemPropertyRepository,
            IWeaponRepository weaponRepository)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.characterRepository = characterRepository;
            this.weaponItemPropertyRepository = weaponItemPropertyRepository;
            this.weaponRepository = weaponRepository;
        }
        
        public async Task<CharacterDto> GetCharacterAsync()
        {
            var character = await characterRepository.GetCurrentAsync(userService.GetCurrentUser().Id);
            return mapper.Map<CharacterDto>(character);
        }

        public async Task<int> IncreaseBalance(int balance)
        {
            var character = await characterRepository.GetCurrentAsync(userService.GetCurrentUser().Id);
            character.Backpack.Wallet.IncreaseBalance(balance);
            await characterRepository.SaveAsync(character);
            return character.Backpack.Wallet.Balance;
        }
        
        public async Task<int> DecreaseBalance(int balance)
        {
            var character = await characterRepository.GetCurrentAsync(userService.GetCurrentUser().Id);
            character.Backpack.Wallet.DecreaseBalance(balance);
            await characterRepository.SaveAsync(character);
            return character.Backpack.Wallet.Balance;
        }

        public async Task EditCharacter(CharacterDto newCharacter)
        {
            var character = await characterRepository.GetCurrentAsync(userService.GetCurrentUser().Id);
            if (newCharacter.Name != character.Name ) character.Rename(newCharacter.Name);
            if (newCharacter.RaceId != character.RaceId) character.ChangeRace(newCharacter.RaceId);
        }

        public async Task<ICollection<WeaponItemDto>> GetWeapons()
        {
            var character = await characterRepository.GetCurrentAsync(userService.GetCurrentUser().Id);
            if (character == null) throw new ApplicationException("User dont have current character");
            var weaponItems = await weaponItemPropertyRepository.GetWeaponItemByItemIdCollection(
                character
                    .Backpack
                    .Items
                    .Where(e => e.Item.Article.CategoryType == CategoryType.Weapon)
                    .Select(e => e.Item.Id)
                    .ToList());
            var items = weaponItems.Select(e => e.Item.Id).ToList();
            var weapons = await weaponRepository.GetDistinctCollectionByArticles(items);

            var result = weaponItems.Select(e =>
            {
                var currentWeapon = weapons.First(w => w.ArticleId == e.Item.ArticleId);   
                return new WeaponItemDto
                {
                    Item = mapper.Map<ItemDto>(e.Item),
                    IsMasterful = e.IsMasterful,
                    Size = e.Size,
                    Damage = currentWeapon.DamageBySize(e.Size),
                    AdditionalDamages = e.AdditionalDamages,
                    Range = currentWeapon.Range,
                    MultiplierCrit = currentWeapon.MultiplierCrit,
                    CritRange = currentWeapon.CritRange,
                    Ammunition = currentWeapon.Ammunition,
                    DamageTypeList = currentWeapon.DamageTypeList,
                    WeaponType = currentWeapon.WeaponType
                };
            }).ToList();

            return result;
        }
    }
}