using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Repositories;
using Pathfinder.Utils.Paging;
using AutoMapper;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IArticleRepository _productRepository;

        public ProductService(IArticleRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<IPagedList<Article>> SearchArticles(PageSearchArgs args)
        {
            return await _productRepository.SearchAsync(args).ConfigureAwait(false);
        }

        public async Task<Article> GetArticleById(int productId)
        {
            return await _productRepository.GetByIdAsync(productId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Article>> GetArticlesByCategoryId(CategoryType categoryType)
        {
            return await _productRepository
                .GetListByCategoryAsync(categoryType);
        }
        
        public async Task<Article> CreateArticle(string name, string description, decimal? price, decimal? weight, byte categoryType)
        {
            var existingArticle = await _productRepository.GetByName(name).ConfigureAwait(false);
            if (existingArticle != null)
            {
                throw new ApplicationException("Article with this id already exists");
            }

            if (!Enum.IsDefined(typeof(CategoryType), categoryType))
            {
                throw new ApplicationException("categoryType is undefined");
            }

            var newArticle = new Article
            {
                Name = name,
                Description = description,
                Price = price,
                Weight = weight,
                CategoryType = (CategoryType) categoryType
            };

            _productRepository.Add(newArticle);

            return newArticle;
        }
        
        public Task UpdateArticle(Article product)
        {
            _productRepository.Save(product);
            return Task.CompletedTask;
        }

        public Task DeleteArticle(Article product)
        {
            _productRepository.Delete(product);
            return Task.CompletedTask;
        }
    }
}
