using AutoMapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Entities.Product;

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
        }
    }
}
