using AutoMapper;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Items;
using Pathfinder.Application.Mapper;
using Pathfinder.Core.Entities.Product;
using Xunit;

namespace Pathfinder.Tests.Mapping
{
    public class MappingTests
    {
        private readonly IMapper _mapper;

        public MappingTests()
        {
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });
            _mapper = mockMapper.CreateMapper();
        }

        private static Category NewCategory =>
            new() { CategoryType = CategoryType.Weapon, Name = "Weapon", Description = "" };

        private static Article NewArticle =>
            new()
            {
                Name = "Name", Category = NewCategory, Price = 200, Weight = 1,
                Description = "Description"
            };

        [Fact]
        public void CategoryMappingTest()
        {
            var category = NewCategory;
            var categoryDto = _mapper.Map<CategoryDto>(category);
            Assert.IsType<CategoryDto>(categoryDto);
            Assert.Equal(category.Name, categoryDto.Name);
            Assert.Equal(category.Description, categoryDto.Description);
            Assert.Equal((byte)category.CategoryType, categoryDto.CategoryType);
        }

        [Fact]
        public void ArticleMappingTest()
        {
            var article = NewArticle;
            var articleDto = _mapper.Map<ProductDto>(article);
            Assert.IsType<ProductDto>(articleDto);
            Assert.Equal(article.Name, articleDto.Name);
            Assert.Equal(article.Price, articleDto.Price);
        }

        [Fact]
        public void ItemMappingTest()
        {
            var item = new Item() { Article = NewArticle };
            var itemDto = _mapper.Map<ItemDto>(item);
            Assert.IsType<ItemDto>(itemDto);
            Assert.IsType<ProductDto>(itemDto.Product);
        }
    }
}