using AutoMapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Application.Models.Auth.Users;

namespace Pathfinder.Application.Mapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryModel>()
                .ReverseMap();
            CreateMap<Article, ArticleModel>()
                .ReverseMap();
            CreateMap<Character, CharacterDto>()
                .ReverseMap();
            CreateMap<Account, AccountDto>()
                .ReverseMap();
            CreateMap<User, UserModel>()
                .ReverseMap();
        }
    }
}
