using AutoMapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;

namespace Pathfinder.Application.Mapper
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            var map = CreateMap<Category, CategoryModel>()
                .ReverseMap();
        }

    }
}
