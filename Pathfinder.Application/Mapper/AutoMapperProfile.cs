using AutoMapper;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Authentication.Users;
using Pathfinder.Application.DTO.Items;
using Pathfinder.Application.DTO.Shop;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Authentication.User;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Entities.Shop;

namespace Pathfinder.Application.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(m => m.CategoryType,
                    opt => opt.MapFrom(src => (byte)src.CategoryType));
            CreateMap<Product, ProductDto>()
                .ForMember(m => m.CategoryType,
                    opt => opt.MapFrom(src => (byte)src.CategoryType));
            CreateMap<Character, CharacterDto>()
                .ReverseMap();
            CreateMap<Account, AccountDto>()
                .ReverseMap();
            CreateMap<User, UserDto>()
                .ReverseMap();
            CreateMap<Race, RaceDto>();
            CreateMap<Size, RaceSizeDto>();
            CreateMap<Characteristic, CharacteristicDto>()
                .ReverseMap();
            CreateMap<GroupCharacteristic, GroupCharacteristicDto>()
                .ReverseMap();
            CreateMap<Item, ItemDto>();
            CreateMap<Shop, ShopDto>();
            CreateMap<Wallet, WalletDto>();
            CreateMap<Backpack, BackpackDto>();
        }
    }
}