using AutoMapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;

namespace Pathfinder.Application.Mapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryModel>()
                .ReverseMap();
            CreateMap<Product, ProductModel>()
                .ReverseMap();
        }

    }
}
