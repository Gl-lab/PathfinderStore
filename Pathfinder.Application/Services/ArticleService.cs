using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<IPagedList<Product>> SearchArticles(PageSearchArgs args)
        {
            return await _productRepository.SearchAsync(args).ConfigureAwait(false);
        }

        public async Task<Product> GetArticleById(int productId)
        {
            return await _productRepository.GetByIdAsync(productId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Product>> GetArticlesByCategoryId(CategoryType categoryType)
        {
            return await _productRepository
                .GetListByCategoryAsync(categoryType);
        }

        public async Task<Product> CreateArticle(string name, string description, decimal? price, decimal? weight,
            byte categoryType)
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

            var newArticle = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Weight = weight,
                CategoryType = (CategoryType)categoryType
            };

            _productRepository.Add(newArticle);

            return newArticle;
        }

        public Task UpdateArticle(Product product)
        {
            _productRepository.Save(product);
            return Task.CompletedTask;
        }

        public Task DeleteArticle(Product product)
        {
            _productRepository.Delete(product);
            return Task.CompletedTask;
        }
    }
}