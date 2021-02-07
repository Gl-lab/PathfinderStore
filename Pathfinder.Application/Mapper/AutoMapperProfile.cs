using AutoMapper;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Application.DTO.Auth.Users;

namespace Pathfinder.Application.Mapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ReverseMap();
            CreateMap<Article, ArticleDto>()
                .ReverseMap();
            CreateMap<Character, CharacterDto>()
                .ReverseMap();
            CreateMap<Account, AccountDto>()
                .ReverseMap();
            CreateMap<User, UserDto>()
                .ReverseMap();
        }
    }
}
