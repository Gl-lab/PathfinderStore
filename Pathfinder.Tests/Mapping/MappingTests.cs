using System;
using System.Collections;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pathfinder.Application.DTO;
using Pathfinder.Application.DTO.Items;
using Pathfinder.Application.Mapper;
using Xunit;
using Pathfinder.Core.Entities.Product;



namespace Pathfinder.Tests.Mapping
{
    public class MappingTests
    {
        private readonly IMapper mapper;

        public MappingTests()
        {
            
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            mapper = mockMapper.CreateMapper();

        }
        private static Category NewCategory =>
            new() {CategoryType = CategoryType.Weapon, Name = "Weapon", Description = ""};
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
            var categoryDto = mapper.Map<CategoryDto>(category);
            Assert.IsType<CategoryDto>(categoryDto);
            Assert.Equal(category.Name, categoryDto.Name); 
            Assert.Equal(category.Description, categoryDto.Description);
            Assert.Equal((byte)category.CategoryType, categoryDto.CategoryType); 
        }
        
        [Fact]
        public void ArticleMappingTest()
        {
            var article = NewArticle;
            var articleDto = mapper.Map<ArticleDto>(article);
            Assert.IsType<ArticleDto>(articleDto);
            Assert.Equal(article.Name, articleDto.Name);
            Assert.Equal(article.Price, articleDto.Price);
        }
        
        [Fact]
        public void ItemMappingTest()
        {
            var item = new Item() {Article = NewArticle};
            var itemDto = mapper.Map<ItemDto>(item);
            Assert.IsType<ItemDto>(itemDto);
            Assert.IsType<ArticleDto>(itemDto.Article);
        }
    }
}