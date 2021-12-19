using AutoMapper;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Auth.Users;
using Pathfinder.Application.DTO.Items;
using Size = Pathfinder.Core.Entities.Account.Size;
using Pathfinder.Core.Entities.Shop;
using Pathfinder.Application.DTO.Shop;
using Pathfinder.Core.Entities.Authentication.User;

namespace Pathfinder.Application.Mapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(m => m.CategoryType, 
                    opt => opt.MapFrom(src => (byte) src.CategoryType));
            CreateMap<Article, ArticleDto>()
                .ForMember(m => m.CategoryType, 
                    opt => opt.MapFrom(src => (byte) src.CategoryType));
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
        }
    }
}