using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO.Items;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Application.UseCases.Characters;

public class
    GetWeaponsForCurrentCharacterHandler : IRequestHandler<GetWeaponsForCurrentCharacterCommand,
        ICollection<WeaponItemDto>>
{
    private readonly IUserService _userService;
    private readonly ICharacterService _characterService;
    private readonly IWeaponService _weaponService;
    private readonly IMapper _mapper;

    public GetWeaponsForCurrentCharacterHandler(IUserService userService, ICharacterService characterService,
        IWeaponService weaponService, IMapper mapper)
    {
        _userService = userService;
        _characterService = characterService;
        _weaponService = weaponService;
        _mapper = mapper;
    }

    public async Task<ICollection<WeaponItemDto>> Handle(GetWeaponsForCurrentCharacterCommand request,
        CancellationToken cancellationToken)
    {
        var weaponItems = await _characterService.WeaponItemProperty(_userService.GetCurrentUser().Id);
        var items = weaponItems.Select(e => e.Item.Id).ToList();
        var weapons = await _weaponService.WeaponsByProductId(items);
        var result = weaponItems.Select(e =>
        {
            var currentWeapon = weapons.First(w => w.ArticleId == e.Item.ArticleId);
            return new WeaponItemDto
            {
                Item = _mapper.Map<ItemDto>(e.Item),
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