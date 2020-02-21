using AutoMapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;

namespace Pathfinder.Application.Mapper
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            var map = CreateMap<Product, ProductModel>()
                .ReverseMap();
        }

    }
}
